using Microsoft.Azure.Management.Automation;
using Microsoft.Azure.Management.Automation.Models;
using Microsoft.Rest.Azure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Rest;
using Microsoft.Rest.Azure.OData;
using Newtonsoft.Json;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Configuration;
using AzureAutomationFormGenerator.WebUI.Models;

namespace AzureAutomationFormGenerator.WebUI.Repos
{
    public interface ICustomAzureOperations
    {
        /// <summary>
        /// Return Job when status is Completed or Failed
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        Task<Job> WaitForJobCompletion(Job job, int timeoutSeconds);
        
        /// <summary>
        /// Return Job when status is Completed or Failed
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        Task<Job> WaitForJobCompletion(string resourceGroup, string automationAccount, Job job, int timeoutSeconds);

        /// <summary>
        /// Start Runbook and wait for result
        /// </summary>
        /// <param name="resourceGroup"></param>
        /// <param name="automationAccount"></param>
        /// <param name="runbookName"></param>
        /// <param name="jobName">Specify custom unique name. Use other overload of method without jobName to default to random guid</param>
        /// <param name="parameters"></param>
        /// <param name="timeOutSeconds"></param>
        /// <returns></returns>
        Task<Tuple<string, string>> StartRunbookAndReturnResult(string resourceGroup, string automationAccount, string runbookName, string jobName, Dictionary<string, string> parameters, int timeOutSeconds = 300);


        /// <summary>
        /// Start runbook and return output. Job Name defaults to random guid
        /// </summary>
        /// <param name="resourceGroup"></param>
        /// <param name="automationAccount"></param>
        /// <param name="runbookName"></param>
        /// <param name="parameters"></param>
        /// <param name="timeOutSeconds"></param>
        /// <returns></returns>
        Task<Tuple<string, string>> StartRunbookAndReturnResult(string resourceGroup, string automationAccount, string runbookName, Dictionary<string, string> parameters, int timeOutSeconds = 300);
        /// <summary>
        /// Return runbook with specified Resource Group, Automation Account and Runbook Name
        /// </summary>
        /// <param name="resourceGroup"></param>
        /// <param name="automationAccount"></param>
        /// <param name="runbookName"></param>
        /// <returns></returns>
        Task<Job> GetJob(string resourceGroup, string automationAccount, string runbookName);

        /// <summary>
        /// Get dictionary of parameter name as key and its specific parameter setting from RunbookParameterSetting class
        /// </summary>
        /// <param name="resourceGroup"></param>
        /// <param name="automationAccount"></param>
        /// <param name="runbookName"></param>
        /// <returns></returns>
        Task<Dictionary<string, RunbookParameterSetting>> GetRunbookParameterSettings(string resourceGroup, string automationAccount, string runbookName);

    }
}
