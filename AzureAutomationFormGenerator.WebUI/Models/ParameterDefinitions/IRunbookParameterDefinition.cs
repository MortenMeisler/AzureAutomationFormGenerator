using Microsoft.Azure.Management.Automation.Models;
using System.Collections.Generic;

namespace AzureAutomationFormGenerator.WebUI.Models.ParameterDefinitions
{
    public interface IRunbookParameterDefinition
    {
        List<string> SelectionValues { get; set; }
        string DefaultValue { get;}
        string DisplayName { get; }
        bool IsArray { get; }
        bool IsRequired { get;  }
        
    }
}