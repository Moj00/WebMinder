﻿using System.Threading.Tasks;
using System.Web;
using WebMinder.Core.Handlers;

namespace WebMinder.Core.Rules.UrlIsValid
{
    public class UrlIsValidRuleHandler : SingleRuleSetHandler<UrlRequest>
    {
        public UrlIsValidRuleHandler()
        {
            RuleSetName = "URL is valid";

            ErrorDescription = "Invalid url";

            Rule = request =>  CheckUrl().Result;

            InvalidAction = () =>
            {
                var ex = new HttpException(403, $"{RuleSetName}  Url unavailable: {RuleRequest.Url}");
                throw ex;
            };
        }

        private async Task<bool> CheckUrl()
        {
            return await RequestUtility.UrlIsValid(RuleRequest.Url, this.Logger); 
        }
    }
}
