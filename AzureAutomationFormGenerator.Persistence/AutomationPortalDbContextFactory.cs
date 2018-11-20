using AzureAutomationFormGenerator.Persistence.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AzureAutomationFormGenerator.Persistence
{
    public class AutomationPortalDbContextFactory : DesignTimeDbContextFactoryBase<AutomationPortalDbContext>
    {
        protected override AutomationPortalDbContext CreateNewInstance(DbContextOptions<AutomationPortalDbContext> options)
        {
            return new AutomationPortalDbContext(options);
        }


    }
}
