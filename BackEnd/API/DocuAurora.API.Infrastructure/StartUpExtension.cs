﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using DocuAurora.Services.Data;
using DocuAurora.Services.Messaging;
using DocuAurora.Data;
using DocuAurora.Data.Common;
using DocuAurora.Data.Common.Repositories;
using DocuAurora.Data.Repositories;
using DocuAurora.Data.Models.MongoDB;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DocuAurora.Data.Models;
using Microsoft.AspNetCore.Identity;

using Serilog;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq;

namespace DocuAurora.API.Infrastructure
{
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
               options =>
               {
                   options.UseSqlServer(
                   configuration.GetConnectionString("DefaultConnection"),
                   providerOptions => providerOptions.EnableRetryOnFailure(
                       maxRetryCount: 3,
                       maxRetryDelay: TimeSpan.FromSeconds(1),
                       errorNumbersToAdd: new List<int>
                       {
                           4060,   // Cannot open database requested by the login.
                           18456,  // Login failed for user.
                           547,    // The INSERT statement conflicted with the FOREIGN KEY constraint.
                           262,    // CREATE DATABASE permission denied in database.
                           2601,   // Cannot insert duplicate key row in object.
                           8152,   // String or binary data would be truncated.
                           207,    // Invalid column name.
                           102,    // Incorrect syntax near.
                           1205,   // Deadlock detected.
                           3201,   // Cannot open backup device.
                           18452,  // Login failed. The login is from an untrusted domain.
                           233,    // A connection was successfully established with the server, but then an error occurred during the login process.}));
                       }));
                   options.LogTo(
                     filter: (eventId, level) => eventId.Id == CoreEventId.ExecutionStrategyRetrying,
                     logger: (eventData) =>
                     {
                         var retryEventData = eventData as ExecutionStrategyEventData;
                         var exceptions = retryEventData.ExceptionsEncountered;
                         Log.Warning($"Retry #{exceptions.Count} with delay {retryEventData.Delay} due to error: {exceptions.Last().Message}");

                     });

               });

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

            Log.Logger = logger;

            return builder;
        }

        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
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
    }
}
