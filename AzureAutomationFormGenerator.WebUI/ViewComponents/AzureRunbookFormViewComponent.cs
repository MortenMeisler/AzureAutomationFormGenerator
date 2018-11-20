using AzureAutomationFormGenerator.WebUI.Models;
using AzureAutomationFormGenerator.WebUI.Repos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.ViewComponents
{

    public class AzureRunbookFormViewComponent : ViewComponent
    {
        private readonly ICustomAzureOperations _customAzureOperations;
        public AzureRunbookFormViewComponent(ICustomAzureOperations customAzureOperations)
        {
            _customAzureOperations = customAzureOperations;
        }

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
                var errorMessage = $"No runbook in URL defined. Please specify Runbook Name in one of the following formats:<br><br>" +
                    Constants.HelpTipURL;
                return View("ErrorNoInput", new ErrorViewModel { ErrorMessage = errorMessage, RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });

            }
            Dictionary<string,RunbookParameterSetting> runbookParameterSettings;
            try
            {
                runbookParameterSettings = await _customAzureOperations.GetRunbookParameterSettings(resourceGroup, automationAccount, runbookName);
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
