using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Models
{
    /// <summary>
    /// Contains PowerShell settings for a parameter within the Automation Runbook. E.g. [ValidateSet("ListValueA", "ListValueB")] or [Alias("displayname of the parameter")] 
    /// </summary>
    public class RunbookParameterSetting
    {
        /// <summary>
        /// if an [ValidateSet("something1","something2")] is defined in the PowerShell Automation Runbook it will be used as list values on a dropdown list for input field
        /// </summary>
        public string[] ValidateSet { get; set; }

        /// <summary>
        /// If an [Alias("something")] is defined in the PowerShell Automation Runbook it will be used as Display Name for the input field. In case more [Alias()] is defined then the first (top) one will be used as Display Name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// If [Mandatory=$true] is defined in the PowerShell Automation Runbook it will be set to true for the input field
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// If parameter has a default value in the PowerShell Automation Runbook it will be set for the input field
        /// </summary>
        public string DefaultValue { get; set; }
    }
}
