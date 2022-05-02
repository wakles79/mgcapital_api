using AutoMapper;
using MGCap.Business.Implementation.Extensions;
using MGCap.DataAccess.Implementation.Context;
using MGCap.DataAccess.Implementation.Extensions;
using MGCap.Domain.Models;
using MGCap.Domain.Options;
using MGCap.Presentation.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;

namespace MGCap.Presentation
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


            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            // Gets Current Connection String (Could be more that one)
            var cnnStr = this.Configuration.GetConnectionString("DefaultConnection");

            // Current Assembly where lives DBContexts
            var asmName = "MGCap.DataAccess.Implementation";

            // Add Identity DB Context
            services.AddDbContext<MGCapIdentityDbContext>(options =>
                options.UseSqlServer(cnnStr, b => b.MigrationsAssembly(asmName)));

            // Add DB Context
            services.AddDbContext<MGCapDbContext>(options =>
                options.UseSqlServer(cnnStr, b => b.MigrationsAssembly(asmName)));

            // Add Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1; // TODO: Change all this settings
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<MGCapIdentityDbContext>()
                .AddDefaultTokenProviders();

            // add AutoMapper service
            services.AddAutoMapper();

            // Add Http ContextAccessor for custom header purposes
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.ConfigureDataAccessDependencies();
            services.ConfigureBusinessDependencies();

            // Add user resolver service
            //services.AddTransient<IUserResolverService, UserResolverService>();

            // Add Jwt Authentication
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "MGCap API", Version = "v1" });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "MGCap.Presentation.xml");
                c.IncludeXmlComments(filePath);
            });

            services.Configure<JwtOptions>(opts =>
            {
                opts.Key = this.Configuration.GetSection("Jwt:Key").Value;
                opts.Issuer = this.Configuration.GetSection("Jwt:Issuer").Value;
                opts.ExpireMinutes = int.Parse(this.Configuration.GetSection("Jwt:ExpireMinutes").Value);
            });

            services.Configure<SmtpOptions>(opts =>
            {
                opts.Server = this.Configuration.GetSection("Smtp:Server").Value;
                opts.FromDisplay = this.Configuration.GetSection("Smtp:FromDisplay").Value;
                opts.User = this.Configuration.GetSection("Smtp:User").Value;
                opts.Password = this.Configuration.GetSection("Smtp:Password").Value;
                opts.FromEmail = this.Configuration.GetSection("Smtp:FromEmail").Value;
            });

            services.Configure<EmailSenderOptions>(opts =>
            {
                opts.Url = this.Configuration["EmailSenderFunction:Url"];
            });

            services.Configure<OneSignalOptions>(opts =>
            {
                opts.ApiKey = this.Configuration.GetSection("OneSignal:ApiKey").Value;
                opts.AppId = this.Configuration.GetSection("OneSignal:AppId").Value;
            });

            services.Configure<GeocoderOptions>(opts =>
            {
                opts.GoogleGeocoderApiKey = this.Configuration["GoogleAPI:GoogleMapsAPIKey"];
            });

            services.Configure<GoogleStreetViewOptions>(opts =>
            {
                opts.APIKey = Configuration.GetSection("GoogleAPI:GoogleStreetViewAPIKey").Value;
            });

            services.Configure<AzureStorageOptions>(opts =>
            {
                opts.DefaultEndpointsProtocol = Configuration.GetSection("AzureStorage:DefaultEndpointsProtocol").Value;
                opts.AccountName = Configuration.GetSection("AzureStorage:AccountName").Value;
                opts.AccountKey = Configuration.GetSection("AzureStorage:AccountKey").Value;
                opts.EndpointSuffix = Configuration.GetSection("AzureStorage:EndpointSuffix").Value;
                opts.StorageImageBaseUrl = Configuration.GetSection("AzureStorage:StorageBaseUrl").Value;
            });

            services.Configure<MGCapDbOptions>(opts =>
            {
                opts.ConnectionString = cnnStr;
            });

            services.Configure<PDFGeneratorApiOptions>(opts =>
            {
                opts.BaseUrl = Configuration.GetSection("PDFGeneratorApi:BaseUrl").Value;
                opts.Key = Configuration.GetSection("PDFGeneratorApi:Key").Value;
                opts.Secret = Configuration.GetSection("PDFGeneratorApi:Secret").Value;
                opts.Workspace = Configuration.GetSection("PDFGeneratorApi:Workspace").Value;
            });

            services.Configure<FreshdeskOptions>(opts =>
            {
                opts.BaseUrl = Configuration.GetSection("Freshdesk:BaseUrl").Value;
                opts.FreshdeskDomain = Configuration.GetSection("Freshdesk:FreshdeskDomain").Value;
            });

            services.Configure<GmailApiOptions>(opts =>
            {
                opts.ConfigJson = Configuration.GetSection("GmailApiCredentials:StringJson").Value;
                opts.TopicName = Configuration.GetSection("GmailApiCredentials:TopicName").Value;
            });

            #region Serilog

            var logDb = cnnStr;
            var logTable = "ExceptionLogs";
            var serilogColumnOptions = new ColumnOptions();
            serilogColumnOptions.Store.Remove(StandardColumn.Properties);
            serilogColumnOptions.Store.Add(StandardColumn.LogEvent);
            //serilogOptions.LogEvent.DataLength = 2048;
            //serilogOptions.PrimaryKey = serilogOptions.TimeStamp;
            //serilogOptions.TimeStamp.NonClusteredIndex = true;

            Log.Logger = new LoggerConfiguration()
                .WriteTo.MSSqlServer(
                    connectionString: logDb,
                    tableName: logTable,
                    autoCreateSqlTable: true,
                    period: TimeSpan.FromMinutes(1),
                    columnOptions: serilogColumnOptions
                ).CreateLogger();

            #endregion

            // Force lowercase urls
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MGCap API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Add Seeds
                MGCapDbContextExtensions.EnsureSeedData(app);
                MGCapIdentityDbContextExtensions.EnsureSeedData(app);
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            // Use authentication
            app.UseAuthentication();
            app.UseCompanyMiddleware();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "api/{controller}/{action=Index}/{guid:Guid?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HOST")))
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                    //spa.UseAngularCliServer(npmScript: "start");
                }

            });
        }
    }
}
