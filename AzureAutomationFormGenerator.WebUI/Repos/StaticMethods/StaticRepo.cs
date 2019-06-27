using AzureAutomationFormGenerator.WebUI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Management.Automation.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Rest.Azure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Repos
{
    public static class StaticRepo
    {
        public static readonly IHttpContextAccessor httpContextAccessor;
        //public static string RunbookName { get; set; }

        public static string AutomationAccount { get; set; }

        public static string ResourceGroup { get; set; }

        public static string ConnectionId { get; set; }

        public static IConfiguration Configuration { get; set; }
        
        //Determines the type of view to be returned - ex. full width (default) or centered
        public enum PageType
        {
            Default,
            FullWidth,
            Centered
        }
        public static PageType CurrentPageType { get; set; }

        /// <summary>  
        /// Get the cookie  
        /// </summary>  
        /// <param name="key">Key </param>  
        /// <returns>string value</returns>  
        public static string GetCookie(IHttpContextAccessor httpContextAccessor, string key)
        {
            return httpContextAccessor.HttpContext.Request.Cookies[key];
        }
        /// <summary>  
        /// set the cookie  
        /// </summary>  
        /// <param name="key">key (unique indentifier)</param>  
        /// <param name="value">value to store in cookie object</param>  
        /// <param name="expireTime">expiration time</param>  
        public static void SetCookie(IHttpContextAccessor httpContextAccessor, string key, string value, int? expireTimeMinutes)
        {
            CookieOptions option = new CookieOptions();
            if (expireTimeMinutes.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTimeMinutes.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);
            httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, option);
        }
        /// <summary>  
        /// Delete the key  
        /// </summary>  
        /// <param name="key">Key</param>  
        public static void RemoveCookie(IHttpContextAccessor httpContextAccessor, string key)
        {
            httpContextAccessor.HttpContext.Response.Cookies.Delete(key);
        }

        /// <summary>
        /// Responsible for exluding empty input from dictionary input and convert certain types to readable entries for automation engine.
        /// </summary>
        /// <param name="httpContext">current httpcontext from the request</param>
        /// <param name="inputs">form field input values as dictionary</param>
        /// <returns></returns>
        public static Dictionary<string, string> SanitizeInput (HttpContext httpContext, IDictionary<string, string> inputs)
        {
           
            var inputsSanitized = new Dictionary<string, string>();
            var lang = httpContext.Request.GetTypedHeaders().AcceptLanguage[0].Value.ToString(); //.OrderByDescending(x => x.Quality ?? 1);
            var culture = CultureInfo.GetCultureInfo(lang);

            foreach (var input in inputs)
            {
                if (!string.IsNullOrEmpty(input.Value))
                {
                    //handle date
                    DateTime dateResult;
                    var dt = DateTime.TryParse(input.Value, culture, DateTimeStyles.None, out dateResult);
                    if (dt == true)
                    {
                        var dtstring = dateResult.ToString("o", CultureInfo.InvariantCulture);
                        inputsSanitized.Add(input.Key, dtstring);
                    }
                    else
                    {
                        inputsSanitized.Add(input.Key, input.Value);
                    }
                }
            }

            return inputsSanitized;
        }

    }

    
}
