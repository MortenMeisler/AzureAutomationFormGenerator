using Microsoft.Azure.Management.Automation.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Models
{
    public class ResultsModel
    {
       /// <summary>
       /// Output from jobstream of type output
       /// </summary>
        public string JobOutput { get; set; }

        /// <summary>
        /// Output from jobstream of type error
        /// </summary>
        public string JobOutputError { get; set; }

        /// <summary>
        /// Job Status returned by runbook Job
        /// </summary>
        public string JobStatus { get; set; }

        /// <summary>
        /// User Input from form submitted by user
        /// </summary>
        public Dictionary<string, string> JobInputs { get; set; }
        
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
