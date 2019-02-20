using Microsoft.Azure.Management.Automation.Models;
using Microsoft.Rest.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Models
{
    public class AzureRunbookFormViewModel
    {
        public string ResourceGroup { get; set; }
        public string AutomationAccount { get; set; }

        public string RunbookName { get; set; }

        public IList<RunbookSimple> Runbooks {get; set;}

        public ResultsModel ResultsModel { get; set; }

    }
}
