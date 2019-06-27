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
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Diagnostics;
using System.Globalization;


namespace AzureAutomationFormGenerator.WebUI.Controllers
{
    
    [AzureADAuthorize]
    [ResponseCache(Duration = 1, NoStore = true, Location = ResponseCacheLocation.None)]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AutomationPortalDbContext _automationPortalDbContext;

        private readonly string _resourceGroup;
        private readonly string _automationAccount;
      
        private readonly ICustomAzureOperations _customAzureOperations;
        
        private readonly IMessageSender _messageSender;
        private string defaultRunbookName;

        private IHttpContextAccessor _httpContextAccessor;


        public HomeController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ICustomAzureOperations customAzureOperations, AutomationPortalDbContext automationPortalDbContext, IMessageSender messageSender)
        {
            _httpContextAccessor = httpContextAccessor;
            _messageSender = messageSender;
            _automationPortalDbContext = automationPortalDbContext;
            StaticRepo.Configuration = configuration;
            _configuration = configuration;
            _customAzureOperations = customAzureOperations;
            _resourceGroup = _configuration["AzureSettings:ResourceGroup"];
            _automationAccount = _configuration["AzureSettings:AutomationAccount"];

            if (!string.IsNullOrEmpty(_configuration["AzureSettings:RunbookName"]))
            {
                defaultRunbookName = _configuration["AzureSettings:RunbookName"];
            }

        }

        /// <summary>
        /// Return view with Runbook Name specified in the URL. Take Resource Group Name and Automation Account Name from static configuration
        /// </summary>
        public IActionResult Index(PageType? pageType)
        {
            return Index(pageType, defaultRunbookName);
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
        /// Return view with Runbook Name specified
        /// </summary>
        /// <param name="runbookName"></param>
        /// <returns></returns>
        [HttpGet("{resourceGroup}/{automationAccount}/{runbookName}")]
        [ValidateAntiForgeryToken]
        public IActionResult Index(PageType? pageType, [FromRoute]string runbookName, string resourceGroup, string automationAccount)
        {
            
            //Set type of page to return. If nothing is passed set Default pagetype
            pageType = pageType.HasValue ? pageType : PageType.Default;
            CurrentPageType = pageType.GetValueOrDefault();

            //save runbook variables
            StaticRepo.AutomationAccount = automationAccount;
            StaticRepo.ResourceGroup = resourceGroup;

            var azureRunbookFormViewModel = new AzureRunbookFormViewModel
            {
                ResourceGroup = resourceGroup,
                AutomationAccount = automationAccount,
                RunbookName = runbookName,
                Runbook = new RunbookSimple { Name = runbookName }
            };

            return View($"Index{pageType.Value}",azureRunbookFormViewModel);
            
        }

        //POST Jobs/Approve
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(string signalRconnectionId, string runbookDisplayName, string runbookName, Dictionary<string, string> inputs)
        {
            Dictionary<string, string> inputsSanitized = SanitizeInput(HttpContext, inputs);

            _messageSender.ConnectionId = signalRconnectionId;
            await _messageSender.SendMessage(_configuration["Text:OutputMessageJobStarted"]);

            //Initialize viewmodel
            AzureRunbookFormViewModel azureRunbookFormViewModel = new AzureRunbookFormViewModel
            {
                ResourceGroup = _resourceGroup,
                AutomationAccount = _automationAccount,
                RunbookName = runbookName,
                Runbook = new RunbookSimple { Name = runbookName, DisplayName = runbookDisplayName },
                ResultsModel = new ResultsModel() { JobInputs = inputsSanitized }
            };

            //AUDIT LOG - START
            if (Configuration.GetValue<bool>("EnableAuditLogging") == true)
            {
                AuditLog logEntry = new AuditLog
                {
                    RequestName = runbookName,
                    RequestUser = HttpContext.User.Identity.Name,
                    RequestInput = JsonConvert.SerializeObject(inputsSanitized, Formatting.None)

                };
                _automationPortalDbContext.Add(logEntry);

                await _automationPortalDbContext.SaveChangesAsync();
            }
            //AUDIT LOG - END
            
            if (CurrentPageType == PageType.Default)
            {
                //Check session cache for existing runbooks if this is null then retrieve runbooks from azure
                azureRunbookFormViewModel.Runbooks = string.IsNullOrEmpty(HttpContext.Session.GetString("Runbooks")) ? 
                    await _customAzureOperations.GetRunbooks(_resourceGroup, _automationAccount).ConfigureAwait(false) 
                    : JsonConvert.DeserializeObject<IList<RunbookSimple>>(HttpContext.Session.GetString("Runbooks"));
                
                return View($"Result{CurrentPageType}", azureRunbookFormViewModel);
            }

            return View($"Result{CurrentPageType}", azureRunbookFormViewModel);

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var errorModel = new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier };
            // Do NOT expose sensitive error information directly to the client.
            #region snippet_ExceptionHandlerPathFeature
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
            {
                errorModel.ErrorMessage = "File error thrown";
            }
            if (exceptionHandlerPathFeature?.Error is ArgumentNullException && exceptionHandlerPathFeature?.Path == "/")
            {
                errorModel.ErrorMessage = $"Some settings in Appsettings are probably wrong or missing.";
            }
            if (exceptionHandlerPathFeature?.Path == "/index")
            {
                errorModel.ErrorMessage += " from home page";
            }
            else
            {
                errorModel.ExceptionMessage += exceptionHandlerPathFeature.Error.Message;
            }
            #endregion
            return View(errorModel);
        }

    }
}
