using Peace_BankingAPI.Domain.Exceptions;
using Peace_BankingAPI.WebAPI.Models;
using System.Text.Json;

namespace Peace_BankingAPI.WebAPI.Middleware
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger
            )
        {
            _next = next;
            _logger = logger;
        }


        public async Task InvokeAsync (HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AccountNotFoundException ex)
            {
                _logger.LogWarning(ex,
                    "Account not found. Path={Path} TraceId={TraceId}",
                    context.Request.Path, context.TraceIdentifier);

                await WriteErrorAsync(context, StatusCodes.Status404NotFound,
                    type: "account_not_found", message: ex.Message);
            }
            catch (InsufficientFundsException ex)
            {
                _logger.LogWarning(ex,
                    "Insufficient funds. Path={Path} TraceId={TraceId}",
                    context.Request.Path, context.TraceIdentifier);

                await WriteErrorAsync(context, StatusCodes.Status400BadRequest,
                    type: "insufficient_funds", message: ex.Message);
            }
            catch (DomainException ex)
            {
                // Catch-all for domain rule violations that don't have a more specific mapping.
                _logger.LogWarning(ex,
                    "Domain rule violated. Path={Path} TraceId={TraceId}",
                    context.Request.Path, context.TraceIdentifier);

                await WriteErrorAsync(context, StatusCodes.Status400BadRequest,
                    type: "domain_error", message: ex.Message);
            }
            catch (Exception ex)
            {
                // Unhandled — log with full context, do NOT leak internal details to the client.
                _logger.LogError(ex,
                    "Unhandled exception. Method={Method} Path={Path} TraceId={TraceId}",
                    context.Request.Method, context.Request.Path, context.TraceIdentifier);

                await WriteErrorAsync(context, StatusCodes.Status500InternalServerError,
                    type: "server_error",
                    message: "An unexpected error occurred. Please try again later.");
            }
        }


        private static async Task WriteErrorAsync(
            HttpContext context,
            int statusCode,
            string type,
            string message,
            IDictionary<string, string[]>? errors = null)
        {
            // If the response has already started, we cannot change headers/status.
            if (context.Response.HasStarted)
            {
                return;
            }

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var payload = new ApiErrorResponse
            {
                Type = type,
                Message = message,
                StatusCode = statusCode,
                TraceId = context.TraceIdentifier,
                Errors = errors
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
