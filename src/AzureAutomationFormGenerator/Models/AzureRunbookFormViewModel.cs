using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.Models
{
    public class AzureRunbookFormViewModel
    {
        public string ResourceGroup { get; set; }
        public string AutomationAccount { get; set; }

        public string RunbookName { get; set; }

    }
}
