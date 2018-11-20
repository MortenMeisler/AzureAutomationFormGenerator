using Microsoft.Azure.Management.Automation.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Models
{
    public class JobOutput
    {
       
        public string Output { get; set; }

        public  string OutputError { get; set; }

        //public JobStatusWrapper JobStatus { get; set; }

        public string JobStatus { get; set; }
        
    }

    public sealed class JobStatusWrapper
    {
        public static readonly JobStatusWrapper Completed
            = new JobStatusWrapper(JobStatus.Completed);

        public static readonly JobStatusWrapper Failed
            = new JobStatusWrapper(JobStatus.Failed);

        public static readonly JobStatusWrapper Stopped
            = new JobStatusWrapper(JobStatus.Stopped);

        private readonly string description;
        private JobStatusWrapper(string description)
        {
            Debug.Assert(!string.IsNullOrEmpty(description));
            this.description = description;
        }

        public static implicit operator string(JobStatusWrapper status)
            => status.description;
    }
}
