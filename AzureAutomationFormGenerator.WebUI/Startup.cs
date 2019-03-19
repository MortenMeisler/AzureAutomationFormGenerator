using AzureAutomationFormGenerator.WebUI.Repos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AzureAutomationFormGenerator.Persistence;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AzureAutomationFormGenerator.WebUI.Security;
using System;

namespace AzureAutomationFormGenerator.WebUI
{
    public class Startup
    {   
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped<IMessageSender, MessageSender>();
            services.AddScoped<ICustomAzureOperations, CustomAzureOperations>();
            //services.AddTransient<ICustomAzureOperations>(cap => new CustomAzureOperations(Configuration));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddCors();
            services.AddSignalR();
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(60);
                options.Cookie.HttpOnly = true;
            });


            //Authentication & Authorization
            #region AUTHENTICATION / AUTHORICATION

            StaticRepo.Configuration = Configuration;
            
            
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(
                        AzureADPolicies.Name, AzureADPolicies.Build);
                });

            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
             .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            #endregion



            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            

            //Enable Audit logging DbContext
            if (Configuration.GetValue<bool>("EnableAuditLogging") == true)
            {
                string strConnectionString = Configuration.GetConnectionString("AutomationPortalDatabase");
                // option to use hardcoded connectionstring
                //strConnectionString = "Server=localhost\\SQLEXPRESS;Database=AutomationPortal;Trusted_Connection=True;Application Name=AutomationPortal;";
                if (string.IsNullOrEmpty(strConnectionString))
                {
                    throw new System.Exception("ConnectionString not found");
                }

                //Infrastructure
                // Add DbContext using SQL Server Provider

                services.AddDbContext<AutomationPortalDbContext>(options =>
                        options.UseSqlServer(strConnectionString));

            }

            //Add empty DbContext
            services.AddDbContext<AutomationPortalDbContext>();

           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
              
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
                //app.UseHsts();
            }
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", $"ALLOW-FROM {Configuration["AzureSettings:CrossOriginAddress"]}");
                await next();
            });

            app.UseCors(builder =>
                builder.WithOrigins($"{Configuration["AzureSettings:CrossOriginAddress"]}")
                .AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod().AllowCredentials()
                );

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
           
            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                //routes.MapHub<SignalHub>("/signalHub");
                routes.MapHub<MessageSender>("/signalHub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
