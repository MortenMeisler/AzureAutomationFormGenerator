using WebUI.HelperMethods;
using Microsoft.Azure.Management.Automation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static AzureAutomationFormGenerator.WebUI.Models.Constants;

namespace AzureAutomationFormGenerator.WebUI.Models.ParameterDefinitions
{
    public class PowershellRunbookParameterDefinition : RunbookParameterDefinition
    {
        public PowershellRunbookParameterDefinition(RunbookParameter runbookParameter) : base(runbookParameter)
        {
            
        }

        public override void SetParameterType(RunbookParameter runbookParameter)
        {

            switch (runbookParameter.Type)
            {
                case "System.String[]":
                case "System.Management.Automation.PSObject[]":
                case "System.Object[]":
                    ParameterType = ParameterTypes.array;
                    break;
                case "System.DateTime":
                    ParameterType = ParameterTypes.datetime;
                    break;
                case "System.Int32":
                    ParameterType = ParameterTypes.@int;
                    break;
                default:
                    ParameterType = ParameterTypes.@string;
                    break;
            }
           

        }

        /// <summary>
        /// Regex check for [ValidateSet("X","Y","Z")] in the parameter and add the values to ValidateSet property
        /// </summary>
        /// <param name="powershellParameterSettingMatch"></param>
        public override void SetSelectionValues(Match powershellParameterSettingMatch)
        {
            Regex regex = new Regex(RegexPatternsPowershell.ValidateSet, RegexOptions.IgnoreCase);
            MatchCollection matchesValidateSet = regex.Matches(powershellParameterSettingMatch.Value);

            if (matchesValidateSet != null && matchesValidateSet.Count > 0)
            {
                SelectionValues = new List<string>() { "" }; //Initialize with first value empty
                SelectionValues.AddRange((matchesValidateSet[0].Value.Replace("\"", "").Replace("'", "")).Split(",").ToList<string>());
                var decodedDefaultvalue = System.Web.HttpUtility.HtmlDecode(DefaultValue);
                DefaultValue = decodedDefaultvalue == null ? null : decodedDefaultvalue.Replace("\"", "");
            }
        }
        /// <summary>
        /// Regex check for [Alias("Display Name X")] in the parameter and add the value to DisplayName property
        /// </summary>
        /// <param name="powershellParameterSettingMatch"></param>
        public override void SetDisplayName(Match powershellParameterSettingMatch)
        {
            Regex regex = new Regex(RegexPatternsPowershell.Alias, RegexOptions.IgnoreCase);
            MatchCollection matchesAlias = regex.Matches(powershellParameterSettingMatch.Value);

            if (matchesAlias != null && matchesAlias.Count > 0)
            {
                DisplayName = matchesAlias[0].Value.Replace("\"", "").Replace("'", "");
            }
        }

       
    
    }
}
