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
        // language=regex,ExplicitCapture
        public const string ValidateSet = @"(?s)(?<=ValidateSet\()(.*?)(?=\)])";

        /// <summary>
        /// Regex pattern for [Alias()] - if more Alias exist the first one will be used
        /// </summary>
        // language=regex,ExplicitCapture
        public const string Alias = @"(?s)(?<=Alias\()(.*?)(?=\)])";

        /// <summary>
        /// Regex pattern for capturing .NAME in comments on top of script. Case sensitive
        /// </summary>
        // language=regex,ExplicitCapture
        public const string DisplayName = @"(?<=.NAME\s*\r\n)[^\n\r]+";

        /// <summary>
        /// Regex pattern for capturing .DESCRIPTION in comments on top of script. Case sensitive
        /// </summary>
        // language=regex,ExplicitCapture
        public const string Description = @"(?<=.DESCRIPTION\s*\r\n)[^\n\r]+";

    }
}
