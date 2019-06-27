using Microsoft.Azure.Management.Automation.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static AzureAutomationFormGenerator.WebUI.Models.Constants;

namespace AzureAutomationFormGenerator.WebUI.Models.ParameterDefinitions
{
    public interface IRunbookParameterDefinition
    {
        /// <summary>
        /// Specify type of parameter
        /// </summary>
        ParameterTypes ParameterType { get; set; }

        /// <summary>
        /// Selection Values used for dropdown list parameter type. Values are set by SetSelectionValues method
        /// </summary>
        List<string> SelectionValues { get; set; }
        /// <summary>
        /// Default value of parameter.Textfield will contain this value at start if specified.
        /// </summary>
        string DefaultValue { get;}
        /// <summary>
        /// Friendly displayname name (alias) of parameter
        /// </summary>
        string DisplayName { get; }
       
        /// <summary>
        /// Determines if the parameter is mandatory or not
        /// </summary>
        bool IsRequired { get;  }

        /// <summary>
        /// Responsible for populating string values to SelectionValues list used by dropdown parameter type
        /// </summary>
        /// <param name="parameterSettingMatch">Regex match for the textblock containing all parameter metadata settings, such as type, set og values etc.</param>
        void SetSelectionValues(Match parameterSettingMatch);

        /// <summary>
        /// Responsible for setting the friendly displayname of the parameter instead of the internal parameter variable name, unless only internal name is found.
        /// </summary>
        /// <param name="parameterSettingMatch">Regex match for the textblock containing all parameter metadata settings, such as type, set og values etc.</param>
        void SetDisplayName(Match parameterSettingMatch);

    }
}