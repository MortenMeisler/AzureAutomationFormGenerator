using AzureAutomationFormGenerator.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace AzureAutomationFormGenerator.Persistence.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {

            builder.Property<int>("AuditLogId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            builder.HasKey("AuditLogId");

            builder.Property<DateTime>("CreatedDate");

            builder.Property<string>("RequestInput");

            builder.Property<string>("RequestUser");

            

            builder.ToTable("AuditLogs");
       

        }
    }
}
