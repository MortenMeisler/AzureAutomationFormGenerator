using AzureAutomationFormGenerator.Persistence.Extensions;
using AzureAutomationFormGenerator.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace AzureAutomationFormGenerator.Persistence
{
    public class AutomationPortalDbContext : DbContext
    {
        public AutomationPortalDbContext()
        {

        }
        public AutomationPortalDbContext(DbContextOptions<AutomationPortalDbContext> options)
           : base(options)
        {
        }

        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyAllConfigurations();
        }
    }
}
