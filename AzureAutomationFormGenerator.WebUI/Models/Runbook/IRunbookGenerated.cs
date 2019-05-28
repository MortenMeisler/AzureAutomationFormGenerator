using System.Collections.Generic;
using AzureAutomationFormGenerator.WebUI.Models.ParameterDefinitions;

namespace AzureAutomationFormGenerator.WebUI.Models.Runbook
{
    public interface IRunbookGenerated
    {
        string Description { get; set; }
        string DisplayName { get; set; }
        string Instruction { get; set; }
        string Name { get; set; }
        IDictionary<string, IRunbookParameterDefinition> ParameterDefinitions { get; set; }
    }
}