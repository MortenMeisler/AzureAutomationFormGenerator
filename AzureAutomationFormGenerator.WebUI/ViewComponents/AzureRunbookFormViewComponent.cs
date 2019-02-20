using AzureAutomationFormGenerator.WebUI.Models;
using AzureAutomationFormGenerator.WebUI.Repos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureAutomationFormGenerator.WebUI.Models.ParameterDefinitions;
using Microsoft.Extensions.Configuration;

namespace AzureAutomationFormGenerator.ViewComponents
{

    public class AzureRunbookFormViewComponent : ViewComponent
    {
        private readonly ICustomAzureOperations _customAzureOperations;
        
        public AzureRunbookFormViewComponent(ICustomAzureOperations customAzureOperations)
        {
            _customAzureOperations = customAzureOperations;
            
        }

        //TODO Text should be put elsewhere
        public async Task<IViewComponentResult> InvokeAsync(string resourceGroup, string automationAccount, string runbookName)
        {
            if (string.IsNullOrEmpty(resourceGroup))
            {
                var errorMessage = $"No Resource Group defined. Please set either a static resource group in appsettings on the server or define one in the URL like this<br><br>" +
                    Constants.HelpTipURLParametersAll;
                return View("ErrorNoInput", new ErrorViewModel {ErrorMessage = errorMessage, RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            if (string.IsNullOrEmpty(automationAccount))
            {
                var errorMessage = $"No Automation Account defined. Please set either a static resource group in appsettings on the server or define one in the URL like this<br><br>" +
                    $"{Constants.HelpTipURLParametersAll}";

                return View("ErrorNoInput", new ErrorViewModel { ErrorMessage = errorMessage, RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            if (string.IsNullOrEmpty(runbookName))
            {
                var errorMessage = $"Select either a runbook from the left table which is populated by the following Tag in Azure: Key: AFK Value: Public or fetch runbook directly from URL<br><br>" +
                //var errorMessage = $"No runbook in URL defined. Please specify Runbook Name in one of the following formats:<br><br>" +
                    Constants.HelpTipURL
                + $"You can hide left table by adding ?pageType=x at the end of the URL. pageType=1 is full width page, pagetype=2 is centered. Example: {Constants.HelpTipURLParametersRunbookOnly}?pageType=2";
                //return View("LandingPage" );
                return View("ErrorNoInput", new ErrorViewModel { ErrorMessage = errorMessage, RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });

            }
            Dictionary<string,IRunbookParameterDefinition> runbookParameterSettings;
            try
            {
                runbookParameterSettings = await _customAzureOperations.GetRunbookParameterDefinitions(resourceGroup, automationAccount, runbookName);
            }catch(Exception ex)
            {

                var errorMessage = $"{ex.Message}<br><br>" +
                    $"Make sure Runbook Name is correct. Please specify Runbook Name in one of the following formats:<br><br>" +
                    Constants.HelpTipURL;
                return View("~/Views/Shared/Error.cshtml", new ErrorViewModel { ErrorMessage = errorMessage, RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });

            }

            ViewBag.RunbookName = runbookName;


            return View(runbookParameterSettings);
        }
    }
}
