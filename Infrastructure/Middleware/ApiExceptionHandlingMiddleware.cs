//  bắt toàn bộ lỗi chưa xử lý trong ứng dụng, quy chuẩn response lỗi không rõ s tacktrace
using Microsoft.EntityFrameworkCore;
using RMS.Contants;

namespace RMS.Infrastructure.Middleware;

public class ApiExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionHandlingMiddleware> _logger;

    public ApiExceptionHandlingMiddleware(RequestDelegate next, ILogger<ApiExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            DbUpdateException dbEx => HandleDbUpdateException(dbEx, context),
            OperationCanceledException => new ServiceResult(false, StatusCodes.Status408RequestTimeout, "Request timeout"),
            _ => new ServiceResult(false, StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };
    
        context.Response.StatusCode = response.Code;
        return context.Response.WriteAsJsonAsync(response);
    }

    private static ServiceResult HandleDbUpdateException(DbUpdateException ex, HttpContext context)
    {
        if (ex.InnerException is Npgsql.PostgresException pgEx)
        {
            if (pgEx.SqlState == "23505") // Unique violation
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                return new ServiceResult(false, StatusCodes.Status409Conflict, "A record with the same unique value already exists");
            }

            if (pgEx.SqlState == "23503") // Foreign key violation
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new ServiceResult(false, StatusCodes.Status400BadRequest, "Related record not found");
            }
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return new ServiceResult(false, StatusCodes.Status500InternalServerError, "Database operation failed");
    }

}