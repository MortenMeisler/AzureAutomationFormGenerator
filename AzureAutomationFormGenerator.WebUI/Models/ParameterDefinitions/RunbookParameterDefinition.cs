using Microsoft.Azure.Management.Automation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static AzureAutomationFormGenerator.WebUI.Models.Constants;

namespace AzureAutomationFormGenerator.WebUI.Models.ParameterDefinitions
{
    public abstract class RunbookParameterDefinition : IRunbookParameterDefinition
    {
        public ParameterTypes ParameterType { get; set; }
        public List<string> SelectionValues { get; set; }
        public RunbookParameterDefinition(RunbookParameter runbookParameter)
        {
            SetParameterType(runbookParameter);
            IsRequired = (bool)runbookParameter.IsMandatory;
            SetDefaultValue(runbookParameter);
        }

        public string DisplayName { get; set; }
        public bool IsRequired { get; private set; }

        public string DefaultValue { get; set; }

        public abstract void SetParameterType(RunbookParameter runbookParameter);

        public abstract void SetDisplayName(Match parameterSettingMatch);

        public void SetDefaultValue(RunbookParameter runbookParameter)
        {
            if (runbookParameter.DefaultValue != null)
            {
                DefaultValue = runbookParameter.DefaultValue.Replace("'", "");
            };

        }

        public abstract void SetSelectionValues(Match parameterSettingMatch);
        
    }

}
