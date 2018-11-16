using AzureAutomationFormGenerator.Models;
using AzureAutomationFormGenerator.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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
using static AzureAutomationFormGenerator.Repos.StaticRepo;

namespace AzureAutomationFormGenerator.Controllers
{

    [ResponseCache(Duration = 1, NoStore = true, Location = ResponseCacheLocation.None)]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private string _resourceGroup;
        private string _automationAccount;
      
        private readonly ICustomAzureOperations _customAzureOperations;
        public string htmlInput;
        private Tuple<string, string> _results;


        private readonly IHubContext<SignalHub> _signalHubContext;

        //TODO: Feedback when submitted immediately - spinning wheel or something or go to next page

        public HomeController(IConfiguration configuration, ICustomAzureOperations customAzureOperations, IHubContext<SignalHub> signalHubContext)
        {
           
            _signalHubContext = signalHubContext;
            StaticRepo.Configuration = configuration;
            StaticRepo.HubContext = signalHubContext;
            _configuration = configuration;
            _customAzureOperations = customAzureOperations;
            _resourceGroup = _configuration["AzureSettings:ResourceGroup"];
            _automationAccount = _configuration["AzureSettings:AutomationAccount"];

            if (!string.IsNullOrEmpty(_configuration["AzureSettings:RunbookName"]))
            {
               StaticRepo.RunbookName = _configuration["AzureSettings:RunbookName"];
            }
           
        }

        /// <summary>
        /// Return view with Runbook Name specified in the URL. Take Resource Group Name and Automation Account Name from static configuration
        /// </summary>
        public async Task<IActionResult> Index(PageType? pageType)
        {
            return await Index(pageType, StaticRepo.RunbookName);
        }

        /// <summary>
        /// Return view with Runbook Name specified in the URL. Take Resource Group Name and Automation Account Name from static configuration
        /// </summary>
        /// <param name="runbookName"></param>
        /// <returns></returns>
        [HttpGet("{runbookName}")]
        public async Task<IActionResult> Index(PageType? pageType, string runbookName)
        {
            return await Index(pageType, runbookName, _resourceGroup, _automationAccount);
        }

        /// <summary>
        /// Return view with Runbook Name specified in the URL. Take Resource Group Name and Automation Account Name from static configuration
        /// </s1ummary>
        /// <param name="runbookName"></param>
        /// <returns></returns>
        /// 
        
        [HttpGet("{resourceGroup}/{automationAccount}/{runbookName}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PageType? pageType, [FromRoute]string runbookName, string resourceGroup, string automationAccount)
        {
            //Set type of page to return. If nothing is passed set Full Width as default
            pageType = pageType.HasValue ? pageType : PageType.FullWidth;
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
            
            //Start runbook and return output
            _results = await _customAzureOperations.StartRunbookAndReturnResult(_resourceGroup, _automationAccount, StaticRepo.RunbookName, inputs);

            ViewData["JobOutput"] = _results.Item1;
            if (_results.Item2 == JobStatus.Failed)
            {
                
                ViewData["JobOutputError"] = _results.Item1;
            }
            

            return View($"Result{currentPageType}");
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
