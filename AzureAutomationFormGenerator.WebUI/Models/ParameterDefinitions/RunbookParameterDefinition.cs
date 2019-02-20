using Microsoft.Azure.Management.Automation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Models.ParameterDefinitions
{
    public class RunbookParameterDefinition : IRunbookParameterDefinition
    {
        public List<string> SelectionValues { get; set; }
        public RunbookParameterDefinition(RunbookParameter runbookParameter)
        {
            IsRequired = (bool)runbookParameter.IsMandatory;
            SetDefaultValue(runbookParameter);
            

        }

        public string DisplayName { get; set; }
        public bool IsRequired { get; private set; }

        public string DefaultValue { get; set; }

        public bool IsArray { get; set; }

        public void SetIsArray(string notused) => throw new NotImplementedException();

        public void SetDisplayName(string notused) => throw new NotImplementedException();

        public void SetDefaultValue(RunbookParameter runbookParameter)
        {
            if (runbookParameter.DefaultValue != null)
            {
                DefaultValue = runbookParameter.DefaultValue.Replace("'", "");
            };

        }

        public void SetSelectionValues(string notused) => throw new NotImplementedException();
        
    }

}
