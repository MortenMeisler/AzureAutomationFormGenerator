﻿// <auto-generated />
using System;
using AzureAutomationFormGenerator.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AzureAutomationFormGenerator.Persistence.Migrations
{
    [DbContext(typeof(AutomationPortalDbContext))]
    [Migration("20181120144918_initCreate6")]
    partial class initCreate6
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AzureAutomationFormGenerator.Persistence.Models.AuditLog", b =>
                {
                    b.Property<int>("AuditLogId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("RequestInput");

                    b.Property<string>("RequestInput2");

                    b.Property<string>("RequestName");

                    b.Property<string>("RequestUser");

                    b.HasKey("AuditLogId");

                    b.ToTable("AuditLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
