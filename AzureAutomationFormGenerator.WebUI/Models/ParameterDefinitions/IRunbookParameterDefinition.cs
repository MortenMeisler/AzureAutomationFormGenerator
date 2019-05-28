using Microsoft.Azure.Management.Automation.Models;
using System.Collections.Generic;

namespace AzureAutomationFormGenerator.WebUI.Models.ParameterDefinitions
{
    public interface IRunbookParameterDefinition
    {
        /// <summary>
        /// Specified values required for parameter
        /// </summary>
        List<string> SelectionValues { get; set; }
        /// <summary>
        /// Default value of parameter.Textfield will contain this value at start if specified.
        /// </summary>
        string DefaultValue { get;}
        /// <summary>
        /// Friendly alternative name (alias) of parameter
        /// </summary>
        string DisplayName { get; }
        /// <summary>
        /// Determines if the parameter is an array type or not
        /// </summary>
        bool IsArray { get; }
        /// <summary>
        /// Determines if the parameter is mandatory or not
        /// </summary>
        bool IsRequired { get;  }

    }
}