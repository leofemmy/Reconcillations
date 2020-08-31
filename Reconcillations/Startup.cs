using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Reconcillations.Entity;
using Reconcillations.Repository;
using Reconcillations.Services;

namespace Reconcillations
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
            // Register reporting services in an application's dependency injection container.
            services.AddDevExpressControls();

            services.AddRazorPages();

            // Add MVC services.
            // services.AddMvc((x) => x.EnableEndpointRouting = false);

            // Add MVC services.
            services.AddMvc(options => { options.EnableEndpointRouting = false; }).AddDefaultReportingControllers();

            //services.Configure<IISOptions>(options =>
            //{
            //    options.AutomaticAuthentication = false;
            //});
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            //// If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            //// If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            //injection connection string from appsetting
            services.AddSingleton(Configuration);

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<ITransactionRepository, TransactionRepository>();

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddTransient<IEmailSender, EmailSender>();

            //inject anti forgery
            services.AddAntiforgery(options => options.HeaderName = "MY-XSRF-TOKEN");
            //sesssion injection
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddHttpContextAccessor();

            services.AddSession(options =>
            {
                // options.IdleTimeout = TimeSpan.FromMinutes(1);
                //options.CookieName = ".Reconciliation";
            });

            //injcetion of newton soft json for 3.0 reference

            services.AddMvcCore().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseSession();

            // Initialize reporting services.
            app.UseDevExpressControls();

            // ...
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "node_modules")),
                RequestPath = "/node_modules",
                EnableDirectoryBrowsing = true

            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller}/{action=Index}/{id?}");
            //});

            //Pre .NET core 3.0 way of doing things
            //app.UseMvc(routes => {<some routing stuff here>});

            //.NET core 3.0 way
            //app.UseRouting();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages(); //Routes for pages
            //    endpoints.MapControllers(); //Routes for my API controllers
            //});
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute("Default", "{controller=Home}/{action=Index}/{id?}");
            //});
            app.UseMvc();
        }
    }
}
