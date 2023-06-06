namespace DocuAurora.API.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;

    using RabbitMQ.Client;
    using Amazon.S3;

    using DocuAurora.Data;
    using DocuAurora.Data.Common;
    using DocuAurora.Data.Common.Repositories;
    using DocuAurora.Data.Models;
    using DocuAurora.Data.Models.MongoDB;
    using DocuAurora.Data.Repositories;
    using DocuAurora.Services.Data;
    using DocuAurora.Services.Data.Configurations;
    using DocuAurora.Services.Data.Contracts;
    using DocuAurora.Services.Messaging;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using MongoDB.Driver;
    using Serilog;
    using System.Threading.Channels;
    using Amazon.S3;
    using static System.Net.WebRequestMethods;

    public static class StartUpExtension
    {
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {new OpenApiSecurityScheme{Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }}, new List<string>() }
                });
            });

            services.AddAuthentication();

            return services;
        }

        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Application services
            services.AddTransient<IEmailSender>(i =>
          new SendGridEmailSender(configuration.GetValue<string>("SendGridSettings:SendGridApiKey")));
            services.AddTransient<IAdminService, AdminService>();
            services.AddTransient<IAdminService, AdminService>();
            services.AddTransient<IChatGPTService, ChatGPTService>();
            services.AddTransient<AuthService>();
            services.AddTransient<GlobalExceptionHandlingMiddleware>();

            return services;
        }

        public static IServiceCollection ConfigureDataRepositories(this IServiceCollection services)
        {
            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            return services;
        }

        public static IServiceCollection ConfigureMongoDB(this IServiceCollection services, IConfiguration configuration)
        {
            // MongoDB
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

        public static IServiceCollection ConfigureRabittMQ(
                                                             this IServiceCollection services,
                                                             IConfiguration configuration)
        {
            services.AddSingleton<ConnectionFactory>(provider =>
            {
                var connectionFactory = new ConnectionFactory
                {
                    HostName = configuration.GetValue<string>("RabbitMQHostConfiguration:HostName"),
                };
                return connectionFactory;
            });

            services.AddSingleton<IModel>(provider =>
            {
                var connectionFactory = provider.GetRequiredService<ConnectionFactory>();

                var connectionCreation = new Lazy<IConnection>(() => connectionFactory.CreateConnection());
                var channelCreation = new Lazy<IModel>(() => connectionCreation.Value.CreateModel());

                var exchange = configuration.GetValue<string>("RabbitMQExchangeConfiguration:Exchange");
                var queue = configuration.GetValue<string>("RabbitMQQueueConfiguration:Queue");

                channelCreation.Value.ExchangeDeclare(exchange, ExchangeType.Direct);

                channelCreation.Value.QueueDeclare(
                   queue,
                   durable: false,
                   exclusive: false,
                   autoDelete: false,
                   arguments: null);

                var routingKey = configuration.GetValue<string>("RabbitMQRoutingKeyConfiguration:RoutingKey");

                channelCreation.Value.QueueBind(queue, exchange, routingKey);

                return channelCreation.Value;
            });

            // RabittMQ
            services.AddSingleton<IRabbitMQService, RabittMQService>();

            return services;
        }

        public static IServiceCollection ConfigureAWSS3Storage(
                                                      this IServiceCollection services,
                                                      IConfiguration configuration)
        {
            services.AddScoped<S3Service>();

            services.AddDefaultAWSOptions(configuration.GetAWSOptions());

            services.AddAWSService<IAmazonS3>();

            return services;
        }

        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // RabittMQ
            services.ConfigureRabittMQ(configuration);

            // MS SQL DB
            services.ConfigureMSSQLDB(configuration);

            // AWS S3 File Storage
            services.ConfigureAWSS3Storage(configuration);

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
            services.ConfigureApplicationServices(configuration);

            // Swagger configuration
            services.ConfigureSwagger();

            // HealthChecks
            services.AddHealthChecks()
                .AddMySql(configuration["ConnectionStrings:DefaultConnection"]);
                //.AddMongoDb(configuration["DocumentStoreDatabaseSettings:ConnectionString"])
        }
    }
}

