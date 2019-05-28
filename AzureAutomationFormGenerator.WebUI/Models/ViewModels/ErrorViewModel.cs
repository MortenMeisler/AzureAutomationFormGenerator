using System;

namespace AzureAutomationFormGenerator.WebUI.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public string ErrorMessage { get; set; }

        public string ExceptionMessage { get; set; }
        public bool ShowExceptionMessage => !string.IsNullOrEmpty(ExceptionMessage);

        public string Exception { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}