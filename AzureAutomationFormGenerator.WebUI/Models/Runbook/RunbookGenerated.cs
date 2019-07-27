using AzureAutomationFormGenerator.WebUI.Models.ParameterDefinitions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Models.Runbook
{
    /// <summary>
    /// Custom runbook type used for providing parsed parameters, friendly names etc. needed for showing the form of the runbook
    /// </summary>
    public class RunbookGenerated : IRunbookGenerated
    {
        /// <summary>
        /// Internal name of the runbook
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Friendly name of the runbook specified in tag: FormGenerator:Name : <YourName>
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Description of the runbook shown on the side-menu specified in tag: FormGenerator:Description : <YourDescription>
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Instruction of the runbook shown at the top specified in Automation Description option
        /// </summary>
        public string Instruction { get; set; }

        /// <summary>
        /// Hybrid Worker Group if defined in tags on the runbook
        /// </summary>
        public string HybridWorkerGroup { get; set; }

        /// <summary>
        /// A dictionary of the parameter name (key) and its collection of parameter definitions (value)
        /// </summary>
        public IDictionary<string, IRunbookParameterDefinition> ParameterDefinitions { get; set; }
    }
}
