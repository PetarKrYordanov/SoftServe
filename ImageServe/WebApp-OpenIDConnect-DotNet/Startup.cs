namespace ImageServe
{
    using System;
  
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage;  

    using TextAnalyticsTools;
    using TextAnalyticsTools.Contracts;
    using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
    using Microsoft.AspNetCore.Mvc.Infrastructure;

    using AutoMapper;
    using BlobStorageTools;
    using BlobStorageTools.Contracts;
    using ComputerVisionTools;
    using ComputerVisionTools.Contracts;
    using ImageServe.Data;
    using ImageServe.Data.Common;
    using ImageServe.Services;
    using ImageServe.Services.Contracts;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            //services.AddDbContext<ImageServeDbContext>(ServiceLifetime.Transient);


            var dbConnectionString = Configuration.GetConnectionString("DbConnectionString");
            services.AddDbContext<ImageServeDbContext>(options =>
                options.UseLazyLoadingProxies()
                .UseSqlServer(dbConnectionString));


            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddAzureAdB2C(options => Configuration.Bind("Authentication:AzureAdB2C", options))
            .AddCookie();

            //services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ITagService, TagService>();


            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            // Add framework services.
            services.AddMvc();

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.CookieHttpOnly = true;
            });

            services.AddAutoMapper();

            //services
            services.AddScoped(typeof(IRepository<>), typeof(DbRepository<>));


            services.AddHttpContextAccessor();
           

            var storageConnectionString = Configuration.GetConnectionString("AzureStorage");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            services.AddScoped<IBlobStorageService, BlobStorageService>(provider => new BlobStorageService(storageAccount));
            

            services.AddScoped<IImageService, ImageService>();
            
            

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISearchService, SearchService>();

            var textAnalyticsSubscriptionKey = Configuration.GetConnectionString("TextAnalyticsSubscriptionKey");
            var textAnalyticsEndpoint = Configuration.GetConnectionString("TextAnalyticsEndpoint");
            var textAnalyticsClient = new TextAnalyticsClient(new AzureTextAnalyticsCredentials(textAnalyticsSubscriptionKey))
            {
                Endpoint = textAnalyticsEndpoint
            };
            services.AddScoped<ITextAnalyticsService, TextAnalyticsService>(provider => new TextAnalyticsService(textAnalyticsClient));



            var computerVisionSubscriptionKey = Configuration.GetConnectionString("ComputerVisionSubscriptionKey");
            var computerVisionEndpoint = Configuration.GetConnectionString("ComputerVisionEndpoint");
            var computerVisionClient = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(computerVisionSubscriptionKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = computerVisionEndpoint
            };
            services.AddScoped<IComputerVisionService, ComputerVisionService>(provider => new ComputerVisionService(computerVisionClient));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                   name: "areas",
                   template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
               );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
