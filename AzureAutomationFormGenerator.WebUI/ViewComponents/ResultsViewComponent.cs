using AzureAutomationFormGenerator.WebUI.Models;
using AzureAutomationFormGenerator.WebUI.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.ViewComponents
{
    public class ResultsViewComponent : ViewComponent
    {
        private readonly ICustomAzureOperations _customAzureOperations;
        
        public ResultsViewComponent(ICustomAzureOperations customAzureOperations)
        {
            _customAzureOperations = customAzureOperations;
        }
        public async Task<IViewComponentResult> InvokeAsync(string resourceGroup, string automationAccount, RunbookSimple runbook, Dictionary<string, string> inputs)
        {
            //Start runbook and return output
            ResultsModel results = await _customAzureOperations.StartRunbookAndReturnResult(resourceGroup, automationAccount, runbook.Name, inputs).ConfigureAwait(false);
            results.Runbook = runbook;
            return View(results);
        }
    }
        
}
