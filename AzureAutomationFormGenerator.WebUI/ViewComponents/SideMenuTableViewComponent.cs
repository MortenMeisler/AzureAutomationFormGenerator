using AzureAutomationFormGenerator.WebUI.Models;
using AzureAutomationFormGenerator.WebUI.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.ViewComponents
{
    public class SideMenuTableViewComponent : ViewComponent
    {
        private readonly ICustomAzureOperations _customAzureOperations;
        private readonly IConfiguration _configuration;
        public SideMenuTableViewComponent(ICustomAzureOperations customAzureOperations, IConfiguration configuration)
        {
            _customAzureOperations = customAzureOperations;
            _configuration = configuration;
        }
        public async Task<IViewComponentResult> InvokeAsync(string resourceGroup, string automationAccount)
        {
            var runbooksNotLoaded = string.IsNullOrEmpty(HttpContext.Session.GetString("Runbooks"));
            ViewBag.RunbooksTitle = _configuration["Text:RunbooksTitle"];
            IList<RunbookSimple> runbooks;
            if (runbooksNotLoaded)
            {
                //Load runbooks from Azure
                runbooks = await _customAzureOperations.GetRunbooks(resourceGroup, automationAccount).ConfigureAwait(false);

                //Getting descriptions foreach runbook (these are not retrieved from ListRunbooks method) //TODO: only get runbook if LastModified is changed to save some miliseconds)
                await Task.Run(() => Parallel.ForEach(runbooks, runbook =>
                {
                    runbook.Description = _customAzureOperations.GetRunbook(resourceGroup, automationAccount, runbook.Name).GetAwaiter().GetResult().Description;                 
                }));

                var serializedRunbooks = JsonConvert.SerializeObject(runbooks);
                HttpContext.Session.SetString("Runbooks", serializedRunbooks);
            }
            else
            {
                //Runbooks already loaded within the user session (idletimeout set in appsettings) - just get them from session variable
                runbooks = JsonConvert.DeserializeObject<IList<RunbookSimple>>(HttpContext.Session.GetString("Runbooks"));
            }
            if (runbooks == null || runbooks.Count == 0) { ViewBag.NoRunbooks = _configuration["Text:NoRunbooks"]; }
            return View(runbooks);
        }
    }
}
