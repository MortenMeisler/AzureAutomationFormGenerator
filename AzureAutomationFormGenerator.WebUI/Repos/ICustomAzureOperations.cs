using Microsoft.Azure.Management.Automation.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureAutomationFormGenerator.WebUI.Models;
using AzureAutomationFormGenerator.WebUI.Models.ParameterDefinitions;
using Microsoft.Rest.Azure;
using AzureAutomationFormGenerator.WebUI.Models.Runbook;

namespace AzureAutomationFormGenerator.WebUI.Repos
{
    public interface ICustomAzureOperations
    {
        /// <summary>
        /// Gets a list of runbooks of type RunbookSimple with user specified Resource Group Name and Automation Account Name (ex. from URL)
        /// </summary>
        /// <param name="resourceGroupName"></param>
        /// <param name="automationAccountName"></param>
        /// <returns></returns>
        Task<IList<RunbookSimple>> GetRunbooks(string resourceGroupName, string automationAccountName);

        /// <summary>
        /// Return RunbookGenerated type with common properties and parameter definitions. Resource Group Name and Automation Account Name specified from user input (ex. directly in URL)
        /// </summary>
        Task<IRunbookGenerated> GetRunbookGenerated(string resourceGroupName, string automationAccountName, string runbookName);

        /// <summary>
        /// Return Job when status is Completed or Failed
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        Task<Job> WaitForJobCompletion(string resourceGroupName, string automationAccountName, Job job, int timeoutSeconds);

        /// <summary>
        /// Start Runbook and wait for result
        /// </summary>
        /// <param name="resourceGroupName"></param>
        /// <param name="automationAccountName"></param>
        /// <param name="runbookName"></param>
        /// <param name="jobName">Specify custom unique name. Use other overload of method without jobName to default to random guid</param>
        /// <param name="parameters"></param>
        /// <param name="timeOutSeconds"></param>
        /// <returns></returns>
        Task<ResultsModel> StartRunbookAndReturnResult(string resourceGroupName, string automationAccountName, string runbookName, string jobName, Dictionary<string, string> parameters, int timeOutSeconds = 300);

        /// <summary>
        /// Get a runbook by name
        /// </summary>
        /// <param name="resourceGroupName"></param>
        /// <param name="automationAccountName"></param>
        /// <param name="runbookName"></param>
        /// <returns></returns>
        Task<Runbook> GetRunbook(string resourceGroupName, string automationAccountName, string runbookName);

        /// <summary>
        /// Start runbook and return output. Job Name defaults to random guid
        /// </summary>
        /// <param name="resourceGroupName"></param>
        /// <param name="automationAccountName"></param>
        /// <param name="runbookName"></param>
        /// <param name="parameters"></param>
        /// <param name="timeOutSeconds"></param>
        /// <returns></returns>
        Task<ResultsModel> StartRunbookAndReturnResult(string resourceGroupName, string automationAccountName, string runbookName, Dictionary<string, string> parameters, int timeOutSeconds = 300);
        /// <summary>
        /// Return runbook with specified Resource Group, Automation Account and Runbook Name
        /// </summary>
        /// <param name="resourceGroupName"></param>
        /// <param name="automationAccountName"></param>
        /// <param name="runbookName"></param>
        /// <returns></returns>
        Task<Job> GetJob(string resourceGroupName, string automationAccountName, string runbookName);

        /// <summary>
        /// Get dictionary of parameter name as key and its specific parameter setting from RunbookParameterSetting class
        /// </summary>
        /// <param name="resourceGroupName"></param>
        /// <param name="automationAccountName"></param>
        /// <param name="runbookName"></param>
        /// <returns></returns>
        Task<Dictionary<string, IRunbookParameterDefinition>> GetRunbookParameterDefinitions(string resourceGroupName, string automationAccountName, Runbook runbook);


    }
}
