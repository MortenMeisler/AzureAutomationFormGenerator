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
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Repos
{
    public static class StaticRepo
    {
        public static readonly IHttpContextAccessor httpContextAccessor;
        public static string RunbookName { get; set; }

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

    }

    
}
