﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Configuration;
using WebMinder.Core.Builders;
using WebMinder.Core.Rules.ApiKey;
using WebMinder.Core.Rules.IpBlocker;

namespace WebMinder.Core
{
    public class SiteMinderModule : IHttpModule
    {
        public delegate void WebMinderRuleRequestReportedEventHandler(object sender, SiteMinderFailuresEventArgs e);
        public event WebMinderRuleRequestReportedEventHandler RuleRequestReported;
        public static SiteMinder SiteMinder { get; set; }

        public void Init(HttpApplication app)
        {

            app.BeginRequest += AppBeginRequest;
            SiteMinder = SiteMinder.Create()
              // .WithSslEnabled()
               .WithApiKeyValidation()
               .WithIpWhitelist()
               //.WithNoSpam(5, TimeSpan.FromHours(1))
               ;

            SiteMinder.Initialise();
        }

        void AppBeginRequest(object sender, EventArgs eventArgs)
        {
            if (HttpContext.Current.Response.StatusCode != 200) return;
            SiteMinder.ValidateWhiteList();
            if (SiteMinder.AllRulesValid()) return;
            var args = new SiteMinderFailuresEventArgs { Failures = SiteMinder.Failures };
            OnRuleRequestReported(args);
        }

        protected virtual void OnRuleRequestReported(SiteMinderFailuresEventArgs e)
        {
            RuleRequestReported?.Invoke(this, e);
        }

        public void Dispose()
        {

        }
    }

    public class SiteMinderFailuresEventArgs : EventArgs
    {
        public List<Exception> Failures { get; set; }
    }
}