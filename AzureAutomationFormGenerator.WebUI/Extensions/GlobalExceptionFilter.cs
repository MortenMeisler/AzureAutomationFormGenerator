using AzureAutomationFormGenerator.WebUI.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Extensions
{
    public class GlobalExceptionFilter : Controller, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var errorModel = new ErrorViewModel();

            errorModel.ExceptionMessage = context.Exception.Message;
            
          
          
        }
    }
}
