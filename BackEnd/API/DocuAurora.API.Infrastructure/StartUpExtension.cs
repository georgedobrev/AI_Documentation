namespace DocuAurora.API.Infrastructure
{
    using System;
    using System.Text;

    using DocuAurora.Data;
    using DocuAurora.Data.Common;
    using DocuAurora.Data.Common.Repositories;
    using DocuAurora.Data.Models;
    using DocuAurora.Data.Models.MongoDB;
    using DocuAurora.Data.Repositories;
    using DocuAurora.Services.Data;
    using DocuAurora.Services.Messaging;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using MongoDB.Driver;
    using Serilog;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class StartUpExtension
	{
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            services.AddAuthentication();

            return services;
        }

        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            // Application services
            services.AddTransient<IEmailSender, NullMessageSender>();
            services.AddTransient<IAdminService, AdminService>();

            return services;
        }

        public static IServiceCollection ConfigureDataRepositories(this IServiceCollection services)
        {
            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();
            services.AddScoped<IDocumentService, DocumentService>();

            return services;
        }

        public static IServiceCollection ConfigureMongoDB(this IServiceCollection services, IConfiguration configuration)
        {
            // MongoDB
            services.Configure<DocumentStoreDatabaseSettings>(
    configuration.GetSection(nameof(DocumentStoreDatabaseSettings)));
            services.AddSingleton<IDocumentStoreDatabaseSettings>(sp =>
             sp.GetRequiredService<IOptions<DocumentStoreDatabaseSettings>>().Value);
            services.AddSingleton<IMongoClient>(s =>
        new MongoClient(configuration.GetValue<string>("DocumentStoreDatabaseSettings:ConnectionString")));

            return services;
        }

        public static IServiceCollection ConfigureCookie(this IServiceCollection services)
        {
            // cookie enhancing security by protecting against cross-site scripting (XSS) attacks.

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.IdleTimeout = new TimeSpan(1, 0, 0, 0);
            });

            services.Configure<CookiePolicyOptions>(
                options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });
            return services;
        }

        public static IServiceCollection ConfigureMSSQLDB(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(
               options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
        {
            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static WebApplicationBuilder ConfigureSeriLog(this WebApplicationBuilder builder)
        {
            var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            return builder;
        }

        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["JwtSettings:Audience"],
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]))
                };
            }).AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
            });

            return services;
        }

        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // MS SQL DB
            services.ConfigureMSSQLDB(configuration);

            // Identity
            services.ConfigureIdentity();

            // cookie enhancing security by protecting against cross-site scripting (XSS) attacks.
            services.ConfigureCookie();

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddControllers().AddNewtonsoftJson();

            services.AddSingleton(configuration);

            // MongoDB
            services.ConfigureMongoDB(configuration);

            // Data repositories
            services.ConfigureDataRepositories();

            // Application services
            services.ConfigureApplicationServices();

            // Swagger configuration
            services.ConfigureSwagger();
        }
    }
}

