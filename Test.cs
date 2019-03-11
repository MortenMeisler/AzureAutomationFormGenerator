using AzureFunctionSubmitForm.Models;
using AzureFunctionSubmitForm.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Management.Automation.Models;
using AzureFunctionSubmitForm.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace AzureFunctionSubmitForm.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private string _resourceGroup;
        private string _automationAccount;
        private static string _runbookName;
        private static string _queryString;
        private static Dictionary<string, AzureFunctionSubmitForm.Models.RunbookParameterSetting> _runbookParameterSettings;
        private readonly IHubContext<Hub> _hubContext;


        private readonly ICustomAzureOperations _customAzureOperations;
        public string htmlInput;


        public HomeController(IConfiguration Configuration, ICustomAzureOperations customAzureOperations, IHubContext<Hub> hubContext)
        {
            _configuration = Configuration;
            _customAzureOperations = customAzureOperations;
            _resourceGroup = _configuration["AzureSettings:ResourceGroup"];
            _automationAccount = _configuration["AzureSettings:AutomationAccount"];
            _hubContext = hubContext;
        }

        /// <summary>
        /// Return view with Runbook Name specified in the URL. Take Resource Group Name and Automation Account Name from static configuration
        /// </summary>
        /// <param name="runbookName"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string runbookName)
        {
            if (string.IsNullOrEmpty(runbookName))
            {
                ViewBag.NoInput = $"No input in URL defined. Please specify Runbook Name in one of the following formats:<br><br>" +
                    $"http://&ltwebsite&gt/?runbookName=&ltMyRunbookName&gt<br>" +
                    $"http://&ltwebsite&gt/?runbookName=&ltMyRunbookName&gt?resourceGroup=&ltMyResourceGroupName&gt?automationAccount=&ltautomationAccountName&gt";
                return View();
            }
            ViewBag.RunbookName = runbookName;
            _runbookName = runbookName;
            _queryString = this.Request.QueryString.ToUriComponent();
            ViewBag.Response = $"ResourceGroup: {_resourceGroup} Automation Account: {_automationAccount} runbook: {runbookName}";

            _runbookParameterSettings = await _customAzureOperations.GetRunbookParameterSettings(_resourceGroup, _automationAccount, runbookName);

            return View(_runbookParameterSettings);
        }

        /// <summary>
        /// Return view with Resource Group Name, Automation Account Name and Runbook Name specified in the URL
        /// </summary>
        /// <param name="resourceGroupName"></param>
        /// <param name="automationAccountName"></param>
        /// <param name="runbookName"></param>
        /// <returns></returns>
        //public async Task<IActionResult> Index(string resourceGroupName, string automationAccountName, string runbookName)
        //{
        //    ViewBag.Response = $"ResourceGroup: {resourceGroupName} Automation Account: {automationAccountName} runbook: {runbookName}";

        //    return View(await _customAzureOperations.GetRunbookParameterSettings(resourceGroupName, automationAccountName, runbookName));
        //}

        //POST Jobs/Approve
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(Dictionary<string, string> inputs)
        {
            //Invoke signal to all clients
            await _signalHubContext.Clients.All.SendAsync("initSignal");

            return RedirectToAction("Success", inputs);
        }

        public async Task<IActionResult> Success(Dictionary<string, string> inputs)
        {
            //Start runbook and return output
            var results = await _customAzureOperations.StartRunbookAndReturnResult(_resourceGroup, _automationAccount, _runbookName, inputs);

            if (results.Item2 == JobStatus.Completed)
            {
                TempData["JobOutput"] = results.Item1;
            }
            else
            {
                TempData["JobOutputError"] = results.Item1;
            }


            return View("Index", _runbookParameterSettings);
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
            //test
            return response;
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
