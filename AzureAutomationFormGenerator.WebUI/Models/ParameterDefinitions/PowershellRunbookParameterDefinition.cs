using WebUI.HelperMethods;
using Microsoft.Azure.Management.Automation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Models.ParameterDefinitions
{
    public class PowershellRunbookParameterDefinition : RunbookParameterDefinition
    {
        public PowershellRunbookParameterDefinition(RunbookParameter runbookParameter) : base(runbookParameter)
        {
            SetIsArray(runbookParameter);
        }

        public void SetIsArray(RunbookParameter runbookParameter)
        {
            IsArray = runbookParameter.Type == "System.String[]" ||
                      runbookParameter.Type == "System.Management.Automation.PSObject[]" ||
                      runbookParameter.Type == "System.Object[]";
        }

        /// <summary>
        /// Regex check for [ValidateSet("X","Y","Z")] in the parameter and add the values to ValidateSet property
        /// </summary>
        /// <param name="powershellParameterSettingMatch"></param>
        public void SetSelectionValues(Match powershellParameterSettingMatch)
        {
            Regex regex = new Regex(RegexPatternsPowershell.ValidateSet, RegexOptions.IgnoreCase);
            MatchCollection matchesValidateSet = regex.Matches(powershellParameterSettingMatch.Value);

            if (matchesValidateSet != null && matchesValidateSet.Count > 0)
            {
                SelectionValues = new List<string>() { "" }; //Initialize with first value empty
                SelectionValues.AddRange((matchesValidateSet[0].Value.Replace("\"", "").Replace("'", "")).Split(",").ToList<string>());
                DefaultValue = System.Web.HttpUtility.HtmlDecode(DefaultValue).Replace("\"", "");
            }
        }
        /// <summary>
        /// Regex check for [Alias("Display Name X")] in the parameter and add the value to DisplayName property
        /// </summary>
        /// <param name="powershellParameterSettingMatch"></param>
        public void SetDisplayName(Match powershellParameterSettingMatch)
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
