using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using DocuAurora.Services.Data;
using DocuAurora.Services.Messaging;
using DocuAurora.Data;
using DocuAurora.Data.Common;
using DocuAurora.Data.Common.Repositories;
using DocuAurora.Data.Repositories;

namespace DocuAurora.API.Infrastructure
{
	public static class StartupExtension
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
    }
}

