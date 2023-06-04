namespace DocuAurora.API
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using DocuAurora.API;
    using DocuAurora.API.Infrastructure;
    using DocuAurora.API.ViewModels.Administration.Users;
    using DocuAurora.Data;
    using DocuAurora.Data.Common;
    using DocuAurora.Data.Common.Repositories;
    using DocuAurora.Data.Models;
    using DocuAurora.Data.Models.MongoDB;
    using DocuAurora.Data.Repositories;
    using DocuAurora.Data.Seeding;
    using DocuAurora.Services.Data;
    using DocuAurora.Services.Mapping;
    using DocuAurora.Services.Messaging;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using MongoDB.Driver;
    using Serilog;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigurationManager configuration = builder.Configuration;

            // Add services to the container SeriLog.
            builder.ConfigureSeriLog();

            builder.Services.ConfigureServices(builder.Configuration);

            // Authentication + JWT Bearer + addGoogle auth
            builder.Services.ConfigureAuthentication(builder.Configuration);

            var app = builder.Build();
            Configure(app);
            app.Run();

            Log.CloseAndFlush();
        }

        private static void Configure(WebApplication app)
        {
            // Seed data on application startup
            using (var serviceScope = app.Services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
                new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
            }

            app.UseHttpsRedirection();
            app.UseCors(options =>
            {
                options.AllowAnyOrigin();
                options.AllowAnyMethod();
                options.AllowAnyHeader();
            });
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            // SERILOG USARNAME INSERTION
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            app.UseMiddleware<LogUserNameMiddleware>();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

        }
    }
}
