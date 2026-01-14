using System.Net;
using System.Text.Json;

namespace NotificationService.API.middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int code;
            if (exception is UnauthorizedAccessException)
                code = (int)HttpStatusCode.Forbidden;
            else if (exception is KeyNotFoundException)
                code = (int)HttpStatusCode.NotFound;
            else if (exception is InvalidOperationException)
                code = (int)HttpStatusCode.Conflict;
            else
                code = (int)HttpStatusCode.InternalServerError;

            context.Response.StatusCode = code;
            string message;
            if (code != 500)
                message = exception.Message;
            else
                message = "Errore interno del server";
            var response = new
            {
                StatusCode = code,
                Message = message,
                Type = exception.GetType().Name
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
