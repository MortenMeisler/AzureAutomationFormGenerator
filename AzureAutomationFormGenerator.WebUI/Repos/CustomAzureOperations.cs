namespace AzureAutomationFormGenerator.WebUI.Repos
{
    using AzureAutomationFormGenerator.WebUI.Models;
    using AzureAutomationFormGenerator.WebUI.Models.ParameterDefinitions;
    using Microsoft.Azure.Management.Automation;
    using Microsoft.Azure.Management.Automation.Models;
    using Microsoft.Azure.Management.ResourceManager.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Rest;
    using Microsoft.Rest.Azure;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="CustomAzureOperations" />
    /// </summary>
    public class CustomAzureOperations : ICustomAzureOperations
    {
        /// <summary>
        /// Defines the _resourceGroup
        /// </summary>
        private readonly string _resourceGroup;

        /// <summary>
        /// Defines the _automationAccount
        /// </summary>
        private readonly string _automationAccount;

        /// <summary>
        /// Gets the Client
        /// </summary>
        public AutomationClient Client { get; private set; }

        /// <summary>
        /// Defines the _configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Runbook Tag
        /// </summary>
        private readonly KeyValuePair<string, string> _automationTag;

        private readonly IMessageSender _messageSender;
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAzureOperations"/> class.
        /// </summary>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/></param>
        public CustomAzureOperations(IConfiguration configuration, IMessageSender messageSender)
        {
            _configuration = configuration;
            _resourceGroup = _configuration["AzureSettings:ResourceGroup"];
            _automationAccount = _configuration["AzureSettings:AutomationAccount"];
            _messageSender = messageSender;

            _automationTag = new KeyValuePair<string, string>(_configuration["AzureSettings:AutomationTag:Key"], _configuration["AzureSettings:AutomationTag:Value"]);
            Client = new AutomationClient(GetCredentials()) { SubscriptionId = _configuration["AzureSettings:SubscriptionId"] };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAzureOperations"/> class.
        /// </summary>
        /// <param name="automationClient">The automationClient<see cref="AutomationClient"/></param>
        public CustomAzureOperations(AutomationClient automationClient)
        {

            Client = automationClient;
        }

        /// <summary>
        /// Gets Azure Credentials from appsetings.json file via Azure Service Principal
        /// </summary>
        /// <returns></returns>
        private AzureCredentials GetCredentials()
        {
            return SdkContext.AzureCredentialsFactory.FromServicePrincipal(
                    _configuration["AzureSettings:ClientId"],
                    _configuration["ClientSecret"],
                    _configuration["AzureSettings:TenantId"],
                    AzureEnvironment.AzureGlobalCloud)
                    .WithDefaultSubscription(_configuration["AzureSettings:SubscriptionId"]);
        }

        public async Task<IList<RunbookSimple>> GetRunbooks(string resourceGroup, string automationAccount)
        {

            var runbooks = (await Client.Runbook.ListByAutomationAccountAsync(resourceGroup, automationAccount)).Where(x => x.Tags.Contains(_automationTag)).Select(r => new RunbookSimple
            {
                Name = r.Name,
            }).ToList<RunbookSimple>() ;

            return runbooks;
        }

        /// <summary>
        /// Returns a dictionary of parameter settings from the runbook where key is the parameter name and value is parameter setting model
        /// </summary>
        /// <param name="resourceGroup"></param>
        /// <param name="automationAccount"></param>
        /// <param name="runbookName"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, IRunbookParameterDefinition>> GetRunbookParameterDefinitions(string resourceGroup, string automationAccount, string runbookName)
        {            
            //Get runbook
            Runbook runbook = await GetRunbook(resourceGroup, automationAccount, runbookName);

            //Get sorted runbook parameters
            IOrderedEnumerable<KeyValuePair<string, RunbookParameter>> runbookParameters = runbook.Parameters.OrderBy(o => o.Value.Position);

            switch (runbook.RunbookType)
            {
                case RunbookTypeEnum.PowerShell:
                    {
                        return await GetPowershellRunbookParameterDefinitions(resourceGroup, automationAccount, runbookName, runbookParameters);                       
                    }
                case "Python2": //No enum here yet wut
                    {
                        throw new NotSupportedException("Python Script not supported yet aaw :(");
                    }
                default:
                    throw new NotSupportedException($"Script type: {runbook.RunbookType} not supported yet aaw :(");
            }

        }

        public async Task<Dictionary<string, IRunbookParameterDefinition>> GetPowershellRunbookParameterDefinitions(string resourceGroup, string automationAccount, string runbookName, IOrderedEnumerable<KeyValuePair<string, RunbookParameter>> runbookParameters)
        {
            //Create empty dictionary
            Dictionary<string, IRunbookParameterDefinition> PSParameterDefinitions = new Dictionary<string, IRunbookParameterDefinition>();

            //Get runbook powershell content
            //string runbookContent = (await GetContentWithHttpMessagesAsync(resourceGroup, automationAccount, runbookName)).Body;
            var stream = await Client.Runbook.GetContentWithHttpMessagesAsync(resourceGroup, automationAccount, runbookName);
            string runbookContent = "";
            using (StreamReader sr = new StreamReader(stream.Body))
            {
                //This allows you to do one Read operation.
                runbookContent = sr.ReadToEnd();
            }

            int i = 0;
            foreach (KeyValuePair<string, RunbookParameter> runbookParameter in runbookParameters)
            {
                PowershellRunbookParameterDefinition PSParameterDefinition = new PowershellRunbookParameterDefinition(runbookParameter.Value);
                
                string pattern;
                if (i == 0)
                {
                    //First parameter configs - get everything between 'Param(' and '<nameoffirstvariable>'
                    pattern = $@"(?s)(?<=param\()(.*?)(?=\${runbookParameter.Key})";
                }
                else
                {
                    //Subsequent parameter configs - get everything between '<nameofpreviousvariable>' and '<nameofcurrentvariable>'
                    pattern = $@"(?s)(?<=\${runbookParameters.ToList()[i - 1].Key})(.*?)(?=\${runbookParameter.Key})";
                }
                Regex regex = new Regex(pattern,RegexOptions.IgnoreCase);
                MatchCollection paramSettingsMatches = regex.Matches(runbookContent);

                if (paramSettingsMatches != null && paramSettingsMatches.Count > 0)
                {
                    var paramSettings = paramSettingsMatches[0];

                    //ValidateSet - Within the specific parameter config check for ValidateSet and add it to RunbookParameterSetting instance
                    PSParameterDefinition.SetSelectionValues(paramSettings);

                    //Alias - Within the specific parameter config check for Alias and add it to RunbookParameterSetting instance
                    PSParameterDefinition.SetDisplayName(paramSettings);
                }

                //Add to dictionary
                PSParameterDefinitions.Add(runbookParameter.Key, PSParameterDefinition);
                i++;
            }


            return PSParameterDefinitions;
        }

        /// <summary>
        /// Create a new runbook job with specified jobname
        /// </summary>
        /// <param name="resourceGroup"></param>
        /// <param name="automationAccount"></param>
        /// <param name="runbookName"></param>
        /// <param name="jobName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<Job> CreateJob(string resourceGroup, string automationAccount, string runbookName, string jobName, Dictionary<string, string> parameters)
        {
            try
            {
                //Create parameters for job
                JobCreateParameters jobCreateParameters = new JobCreateParameters(new RunbookAssociationProperty
                {
                    Name = runbookName
                }, parameters);

                //Create job
                return await Client.Job.CreateAsync(resourceGroup, automationAccount, jobName, jobCreateParameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Runbook job could not be created. Error: {ex}");
                
            }
        }

        /// <summary>
        /// Start runbook and wait for result. Job Name defaults to random guid
        /// </summary>
        /// <param name="resourceGroup"></param>
        /// <param name="automationAccount"></param>
        /// <param name="runbookName"></param>
        /// <param name="parameters"></param>
        /// <param name="timeOutSeconds"></param>
        /// <returns></returns>
        public async Task<ResultsModel> StartRunbookAndReturnResult(string resourceGroup, string automationAccount, string runbookName, Dictionary<string, string> parameters, int timeOutSeconds = 300)
        {
            return await StartRunbookAndReturnResult(resourceGroup, automationAccount, runbookName, Guid.NewGuid().ToString(), parameters, timeOutSeconds: timeOutSeconds);
        }

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
        public async Task<ResultsModel> StartRunbookAndReturnResult(string resourceGroup, string automationAccount, string runbookName, string jobName, Dictionary<string, string> parameters, int timeOutSeconds = 300)
        { 
            try {
                //Create job
                Job job = await CreateJob(resourceGroup, automationAccount, runbookName,jobName, parameters);

                if (job != null)
                {

                    await _messageSender.SendStatus(job.Status);

                    //Wait for job to complete or fail
                    job = await WaitForJobCompletion(resourceGroup, automationAccount, job, timeOutSeconds);
                
                    //Get job output for failed or something else
                    switch (job.Status)
                    {
                        case JobStatus.Failed:
                            return new ResultsModel() { JobOutputError = job.Exception, JobStatus = JobStatus.Failed, JobInputs = parameters };
                        default:
                            //Get job streams and format into output string
                            IPage<JobStream> jobStreams = await GetJobStreams(resourceGroup, automationAccount, job);
                            return new ResultsModel() { JobOutput = GetJobOutputs(jobStreams), JobStatus = job.Status, JobInputs = parameters };
                    }
                }
                    return null;

            }catch (Exception ex)
            {
                throw new Exception($"Job could not be created for job with name: {jobName} and Runbook: {runbookName}", ex);
            }
            
        }

        /// <summary>
        /// Type of Output to retrieve from stream
        /// </summary>
        public enum JobOuputType
        {
            /// <summary>
            /// Defines the OnlyOutput
            /// </summary>
        OnlyOutput,

            /// <summary>
            /// Defines the OnlyError
            /// </summary>
        OnlyError,

            /// <summary>
            /// Defines the All
            /// </summary>
        All,

        }

        /// <summary>
        /// Get all outputs from runbook (all Write-Output and/or Write-Error/exceptions from output)
        /// Default: Return every type of output (error and output text). Each write-output will be appended with html break tag to support line break
        /// </summary>
        /// <param name="jobStreams">JobStream from runbook</param>
        /// <param name="jobOuputType">Select to only return output, only error or all (default)</param>
        /// <param name="toHTML">Appends html break tag at the end of line, otherwise a new line control string character</param>
        /// <returns></returns>
        private string GetJobOutputs(IPage<JobStream> jobStreams, JobOuputType jobOuputType = JobOuputType.All, bool? toHTML = true)
        {
            StringBuilder sb = new StringBuilder();
            foreach (JobStream output in jobStreams)
            {
                switch (jobOuputType)
                {
                    case JobOuputType.OnlyOutput:
                        if (output.StreamType == StreamType.Output)
                        {
                            sb.Append(GetJobOutput(output, toHTML));
                        }
                        break;
                    case JobOuputType.OnlyError:
                        if (output.StreamType == StreamType.Error)
                        {
                            sb.Append(GetJobOutput(output, toHTML));
                        }
                        break;
                    case JobOuputType.All:
                        sb.Append(GetJobOutput(output, toHTML));
                        break;
                    default:
                        sb.Append(GetJobOutput(output, toHTML));
                        break;
                }
                
            }

            return sb.ToString();
        }

        /// <summary>
        /// Return job output with newline
        /// </summary>
        /// <param name="js"></param>
        /// <param name="toHTML"></param>
        /// <returns></returns>
        private string GetJobOutput(JobStream js, bool? toHTML)
        {
            
            if (toHTML != null && toHTML == true)
            {
                if (js.StreamType == StreamType.Error)
                {
                    return string.Format("{0}", $"<p style='color: red'>{System.Net.WebUtility.HtmlEncode(js.Summary)}</p>");
                }
                else
                {
                    return string.Format("{0}", $"<p>{System.Net.WebUtility.HtmlEncode(js.Summary)}</p>");
                }

                
            }
            else
            {
                return string.Format("{0}{1}", js.Summary, Environment.NewLine);
            }
        }

        /// <summary>
        /// The GetJobStreams
        /// </summary>
        /// <param name="resourceGroup">The resourceGroup<see cref="string"/></param>
        /// <param name="automationAccount">The automationAccount<see cref="string"/></param>
        /// <param name="job">The job<see cref="Job"/></param>
        /// <returns>The <see cref="Task{IPage{JobStream}}"/></returns>
        public async Task<IPage<JobStream>> GetJobStreams(string resourceGroup, string automationAccount, Job job)
        {
            Job completedJob = await WaitForJobCompletion(resourceGroup, automationAccount, job);

            return await Client.JobStream.ListByJobAsync(resourceGroup, automationAccount, completedJob.Name);
        }

        /// <summary>
        /// Wait for job to complete before returning job
        /// </summary>
        /// <param name="resourceGroup">The resourceGroup<see cref="string"/></param>
        /// <param name="automationAccount">The automationAccount<see cref="string"/></param>
        /// <param name="job"></param>
        /// <param name="timeOutSeconds">The timeOutSeconds<see cref="int"/></param>
        /// <returns></returns>
        public async Task<Job> WaitForJobCompletion(string resourceGroup, string automationAccount, Job job, int timeOutSeconds = 600)
        {
           
            Stopwatch sw = new Stopwatch();
            sw.Start();
            do
            {
                if (sw.ElapsedMilliseconds % 200 == 0)
                {
                    job = await GetJob(resourceGroup, automationAccount, job.Name);
                    await _messageSender.SendStatus(job.Status);
                    
                    
                }

                if(job.Status == JobStatus.Completed || job.Status == JobStatus.Failed || job.Status == JobStatus.Blocked 
                    || job.Status == JobStatus.Stopped || job.Status == JobStatus.Suspended || job.Status == JobStatus.Disconnected)
                {
                    sw.Stop();
                }

                if (sw.ElapsedMilliseconds > timeOutSeconds * 1000)
                {
                    sw.Stop();
                    throw new TimeoutException($"Could not get job '{job.Name}'. Timeout after {timeOutSeconds} seconds");
                }

            } while (sw.IsRunning);
            sw.Stop();

            return job;
        }

        /// <summary>
        /// Wait for job to complete before returning job
        /// </summary>
        /// <param name="job"></param>
        /// <param name="timeOutSeconds">The timeOutSeconds<see cref="int"/></param>
        /// <returns></returns>
        public async Task<Job> WaitForJobCompletion(Job job, int timeOutSeconds = 300)
        {
            return await WaitForJobCompletion(_resourceGroup, _automationAccount, job, timeOutSeconds);
        }

        /// <summary>
        /// Return runbook with specified Resource Group, Automation Account and Runbook Name
        /// </summary>
        /// <param name="resourceGroup"></param>
        /// <param name="automationAccount"></param>
        /// <param name="runbookName"></param>
        /// <returns></returns>
        public async Task<Runbook> GetRunbook(string resourceGroup, string automationAccount, string runbookName)
        {
            var runbook = await Client.Runbook.GetAsync(resourceGroup, automationAccount, runbookName);

            //Return runbook only if it adheres to the default tag
            if (runbook.Tags.Contains(_automationTag))
            {
                return runbook;
            }
            else
            {
                throw new Exception($"No proper Tag found on runbook: {runbookName}. Resolve this by placing a Tag. Example: {_automationTag.Key}:{_automationTag.Value}");
            }
           
        }

        /// <summary>
        /// Return Job with specified Job Name and with default Automation Account and Resource Group provided from appsettings configuration file
        /// </summary>
        /// <param name="jobName">The jobName<see cref="string"/></param>
        /// <returns></returns>
        public async Task<Job> GetJob(string jobName)
        {
            return await GetJob(_resourceGroup, _automationAccount, jobName);
        }

        /// <summary>
        /// Get Job by jobname
        /// </summary>
        /// <param name="resourceGroup"></param>
        /// <param name="automationAccount"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public async Task<Job> GetJob(string resourceGroup, string automationAccount, string jobName)
        {
            return await Client.Job.GetAsync(resourceGroup, automationAccount, jobName);
        }

        /// <summary>
        /// Return Job with specified Job Name and with default Automation Account and Resource Group provided from appsettings configuration file
        /// </summary>
        /// <param name="runbookName"></param>
        /// <returns></returns>
        public async Task<Runbook> GetRunbook(string runbookName)
        {
            return await GetRunbook(_resourceGroup, _automationAccount, runbookName);
        }
    }
}
