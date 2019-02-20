using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Models
{
    public static class Constants
    {
        /// <summary>
        /// Regex pattern for [ValidateSet()]
        /// </summary>
        public const string RegexPatternValidateSet = @"(?s)(?<=ValidateSet\()(.*?)(?=\)])";

        /// <summary>
        /// Regex pattern for [Alias()] - if more Alias exist the first one will be used
        /// </summary>
        public const string RegexPatternAlias = @"(?s)(?<=Alias\()(.*?)(?=\)])";



        //public const string HelpTipURLParametersAll = @"http://&ltwebsite&gt/?runbookName=&ltMyRunbookName&gt&resourceGroup=&ltMyResourceGroupName&gt&automationAccount=&ltautomationAccountName&gt";
        public const string HelpTipURLParametersAll = @"https://&ltwebsite&gt/&ltMyResourceGroupName&gt/&ltMyAutomationAccountName&gt/&ltMyRunbookName&gt";

        //public const string HelpTipURLParametersRunbookOnly = @"http://&ltwebsite&gt/?runbookName=&ltMyRunbookName&gt";
        public const string HelpTipURLParametersRunbookOnly = @"https://&ltwebsite&gt/&ltMyRunbookName&gt";

        public static string HelpTipURL = $"If Automation Account and Resource Group is specified in appsettings on the server use this URL format:<br>" +
                    $"{HelpTipURLParametersRunbookOnly}<br><br>" +
                    $"If only subscription is specified in appsettings on the server use this URL format:<br>" +
                    $"{HelpTipURLParametersAll}";

        

    }
}
