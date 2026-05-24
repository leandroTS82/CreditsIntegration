using Credits.Domain.Exceptions;
using FluentValidation;

namespace Credits.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors
                .Select(e => new { field = e.PropertyName, message = e.ErrorMessage });

            await context.Response.WriteAsJsonAsync(new { success = false, errors });
        }
        catch (BusinessException ex)
        {
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Method} {Path}",
               context.Request.Method,
               context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new { success = false, message = "Unexpected error." });
        }
    }
}