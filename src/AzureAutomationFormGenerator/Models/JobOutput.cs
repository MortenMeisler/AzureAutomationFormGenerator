using Microsoft.Azure.Management.Automation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Models
{
    public class JobOutput
    {
       
        public string Output { get; set; }

        public  string OutputError { get; set; }

        //public JobStatus JobStatus { get; set; }
        
    }
}
