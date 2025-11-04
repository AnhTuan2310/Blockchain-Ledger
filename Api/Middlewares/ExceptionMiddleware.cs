using System.Net;
using Api.Models;

namespace Api.Middlewares;

public class ExceptionMiddleware {
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env) {
        _next = next;
        _env = env;
    }

    public async Task Invoke(HttpContext context) {
        try {
            await _next(context);
        } catch (Exception ex) {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Prod mode: không show stack trace
            var message = _env.IsDevelopment()
                ? $"{ex.Message} | {ex.StackTrace}"
                : ex.Message;

            var response = ApiResponse<object>.Failure(message);

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
