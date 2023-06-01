using System;
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
    }
}

