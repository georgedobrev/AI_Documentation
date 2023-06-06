using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using SendGrid.Helpers.Errors.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace DocuAurora.API.Infrastructure
{
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (NotFoundException e)
            {
                _logger.LogError(e, e.Message);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await WriteProblemDetails(context, "Not found", "The requested resource could not be found.");
            }
            catch (ValidationException e)
            {
                _logger.LogError(e, e.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await WriteProblemDetails(context, "Bad request", e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await WriteProblemDetails(context, "Server error", "An internal server error occurred.");
            }
        }

        private static async Task WriteProblemDetails(HttpContext context, string title, string detail)
        {
            ProblemDetails problem = new()
            {
                Status = context.Response.StatusCode,
                Title = title,
                Detail = detail
            };

            string json = JsonConvert.SerializeObject(problem);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }

    }
}