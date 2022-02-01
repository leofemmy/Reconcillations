using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using Hangfire;
using Hangfire.SqlServer;
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
using Serilog;

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

            services.ConfigureReportingServices(configurator =>
            {
                configurator.UseDevelopmentMode();
                configurator.DisableCheckForCustomControllers();
                // ... 
            });


            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));


            // Add the processing server as IHostedService
            services.AddHangfireServer();

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient backgroundJobs)
        {

            // Initialize reporting services.
            app.UseDevExpressControls();

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


            app.UseStaticFiles();
            // ...
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "node_modules")),
                RequestPath = "/node_modules",
                EnableDirectoryBrowsing = true

            });



            //"/hangfire"

            app.UseHangfireDashboard("/Pusher");

            RecurringJob.AddOrUpdate(() => DoReemsPush(), Cron.MinuteInterval(2));

            app.UseHttpsRedirection();



            app.UseSession();



            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute( //<-- ADDED
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
            });


        }

        public void DoReemsPush()
        {
            //var connectionString = this.GetConnection();

            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
            try
            {
                SqlDataAdapter _adp;

                DataSet response = new DataSet();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new SqlCommand("spdoGetRecordtoReems", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);


                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Reems Push To Successfully ");

                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();

                logger.Fatal($"Do Reems Push thrown an error - {e.Message}");
            }
        }
    }
}
