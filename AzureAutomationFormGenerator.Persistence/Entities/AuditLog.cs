using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAutomationFormGenerator.Persistence.Models
{
    public class AuditLog
    {

        //public AuditLog()
        //{
        //    AuditLogs = new HashSet<AuditLog>();
        //}

        public int AuditLogId { get; set; }

        public string RequestName { get; set; }
        public string RequestUser { get; set; }

        public string RequestInput { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        
        
    }
}
