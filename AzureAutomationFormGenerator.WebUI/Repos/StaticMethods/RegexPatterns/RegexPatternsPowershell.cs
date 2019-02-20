using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.HelperMethods
{
    public static class RegexPatternsPowershell
    {
        /// <summary>
        /// Regex pattern for [ValidateSet()]
        /// </summary>
        public const string ValidateSet = @"(?s)(?<=ValidateSet\()(.*?)(?=\)])";

        /// <summary>
        /// Regex pattern for [Alias()] - if more Alias exist the first one will be used
        /// </summary>
        public const string Alias = @"(?s)(?<=Alias\()(.*?)(?=\)])";

    }
}
