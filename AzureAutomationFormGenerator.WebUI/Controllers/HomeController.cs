using AzureAutomationFormGenerator.WebUI.Models;
using AzureAutomationFormGenerator.WebUI.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Management.Automation.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using static AzureAutomationFormGenerator.WebUI.Repos.StaticRepo;
using AzureAutomationFormGenerator.Persistence;
using AzureAutomationFormGenerator.Persistence.Models;
using Microsoft.AspNetCore.Authorization;
using AzureAutomationFormGenerator.WebUI.Security;
using Microsoft.Rest.Azure;

namespace AzureAutomationFormGenerator.WebUI.Controllers
{
    //[Authorize(Policy = "ADAuthorizationRequired")]
    [AzureADAuthorize]
    [ResponseCache(Duration = 1, NoStore = true, Location = ResponseCacheLocation.None)]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AutomationPortalDbContext _automationPortalDbContext;

        private string _resourceGroup;
        private string _automationAccount;
      
        private readonly ICustomAzureOperations _customAzureOperations;
        public string htmlInput;
        private AzureRunbookFormViewModel _azureRunbookFormViewModel;
        private KeyValuePair<string, string> _automationTag;


        private readonly IHubContext<SignalHub> _signalHubContext;

        //TODO: Feedback when submitted immediately - spinning wheel or something or go to next page

        public HomeController(IConfiguration configuration, ICustomAzureOperations customAzureOperations, IHubContext<SignalHub> signalHubContext, AutomationPortalDbContext automationPortalDbContext)
        {

            _automationPortalDbContext = automationPortalDbContext;
            _signalHubContext = signalHubContext;
            StaticRepo.Configuration = configuration;
            StaticRepo.HubContext = signalHubContext;
            _configuration = configuration;
            _customAzureOperations = customAzureOperations;
            _resourceGroup = _configuration["AzureSettings:ResourceGroup"];
            _automationAccount = _configuration["AzureSettings:AutomationAccount"];
            _automationTag = new KeyValuePair<string, string>(_configuration["AzureSettings:AutomationTag:Key"], _configuration["AzureSettings:AutomationTag:Value"]);

            if (!string.IsNullOrEmpty(_configuration["AzureSettings:RunbookName"]))
            {
                StaticRepo.RunbookName = _configuration["AzureSettings:RunbookName"];
            }

        }

        /// <summary>
        /// Return view with Runbook Name specified in the URL. Take Resource Group Name and Automation Account Name from static configuration
        /// </summary>
        public IActionResult Index(PageType? pageType)
        {
            return Index(pageType, StaticRepo.RunbookName);
        }

        /// <summary>
        /// Return view with Runbook Name specified in the URL. Take Resource Group Name and Automation Account Name from static configuration
        /// </summary>
        /// <param name="runbookName"></param>
        /// <returns></returns>
        [HttpGet("{runbookName}")]
        public IActionResult Index(PageType? pageType, string runbookName)
        {
            return Index(pageType, runbookName, _resourceGroup, _automationAccount);
        }

        /// <summary>
        /// Return view with Runbook Name specified in the URL. Take Resource Group Name and Automation Account Name from static configuration
        /// </s1ummary>
        /// <param name="runbookName"></param>
        /// <returns></returns>
        /// 
        
        [HttpGet("{resourceGroup}/{automationAccount}/{runbookName}")]
        [ValidateAntiForgeryToken]
        public IActionResult Index(PageType? pageType, [FromRoute]string runbookName, string resourceGroup, string automationAccount)
        {
            //Set type of page to return. If nothing is passed set Full Width as default
            pageType = pageType.HasValue ? pageType : PageType.Default;
            currentPageType = pageType.GetValueOrDefault();

            //save runbook variables
            StaticRepo.RunbookName = runbookName;
            StaticRepo.AutomationAccount = automationAccount;
            StaticRepo.ResourceGroup = resourceGroup;

            var azureRunbookFormViewModel = new AzureRunbookFormViewModel
            {
                ResourceGroup = resourceGroup,
                AutomationAccount = automationAccount,
                RunbookName = runbookName
            };
           
            _azureRunbookFormViewModel = azureRunbookFormViewModel;



            return View($"Index{pageType.Value}",azureRunbookFormViewModel);
            
        }

       

        //POST Jobs/Approve
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(string signalRconnectionId, Dictionary<string, string> inputs)
        {
            StaticRepo.ConnectionId = signalRconnectionId;

            //Initialize viewmodel
            AzureRunbookFormViewModel azureRunbookFormViewModel = new AzureRunbookFormViewModel
            {
                ResourceGroup = _resourceGroup,
                AutomationAccount = _automationAccount,
                RunbookName = RunbookName,
                ResultsModel = new ResultsModel() { JobInputs = inputs}
            };

            //AUDIT LOG - START
            if (Configuration.GetValue<bool>("EnableAuditLogging") == true)
            {
                AuditLog logEntry = new AuditLog
                {
                    RequestName = StaticRepo.RunbookName,
                    RequestUser = HttpContext.User.Identity.Name,
                    RequestInput = JsonConvert.SerializeObject(inputs, Formatting.None)

                };
                _automationPortalDbContext.Add(logEntry);

                await _automationPortalDbContext.SaveChangesAsync();
            }
            //AUDIT LOG - END

            //Start runbook and return output
            //ResultsModel _results = await _customAzureOperations.StartRunbookAndReturnResult(_resourceGroup, _automationAccount, StaticRepo.RunbookName, inputs).ConfigureAwait(false);

            //if (_results.JobStatus == JobStatus.Failed)
            //{
                
            //    ViewData["JobOutputError"] = _results.Item1;
            //    ViewData["JobInput"] = inputs;
            //}
            //else
            //{
            //    ViewData["JobOutput"] = _results.Item1;
            //}
            
            if (currentPageType == PageType.Default)
            {
                //Check session cache for existing runbooks if this is null then retrieve runbooks from azure
                azureRunbookFormViewModel.Runbooks = string.IsNullOrEmpty(HttpContext.Session.GetString("Runbooks")) ? 
                    await _customAzureOperations.GetRunbooks(_automationTag, _resourceGroup, _automationAccount).ConfigureAwait(false) 
                    : JsonConvert.DeserializeObject<IList<RunbookSimple>>(HttpContext.Session.GetString("Runbooks"));
                
                return View($"Result{currentPageType}", azureRunbookFormViewModel);
            }

            return View($"Result{currentPageType}", azureRunbookFormViewModel);



        }

        



        private WebResponse RunWebHook(string webHookUri, string input)
        {
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create(new Uri(webHookUri));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";

            string parsedContent = input;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            WebResponse response = http.GetResponse();

            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            return response;
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
