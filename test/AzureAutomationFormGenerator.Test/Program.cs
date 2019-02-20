using System;
using AzureAutomationFormGenerator.WebUI.Repos;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Collections.Generic;
using System.Web;

namespace AzureAutomationFormGenerator.Test
{
    class Program
    {
        private static ICustomAzureOperations _customAzureOperations;
      
        static void Main(string[] args)
        {
            Console.WriteLine("Let's do some unit test no?");
            string[] array = new string[] { "hej" };
            string ok = "Hej2";
            var decoded = HttpUtility.HtmlDecode("\"Hej2\"");
            decoded = decoded.Replace("\"", "");
            Console.WriteLine(decoded);
            Console.WriteLine(decoded.Equals(ok));


            Console.ReadKey();
        }
    }

}
