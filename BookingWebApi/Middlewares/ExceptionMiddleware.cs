using System.Net;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApi.Error;

namespace WebApi.Middlewares
{
    public class ExceptionMiddleware
    {
        public readonly RequestDelegate next;
        public readonly ILogger<ExceptionMiddleware> logger;
        public readonly IHostEnvironment env;
        public ExceptionMiddleware(RequestDelegate next,
                                   ILogger<ExceptionMiddleware> logger,
                                   IHostEnvironment env)
        {
            this.env = env;
            this.logger = logger;
            this.next = next;

        }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            ApiError response;
            HttpStatusCode statusCode;
            string message;
            var exceptionType = ex.GetType();
            if(exceptionType == typeof(UnauthorizedAccessException))
            {
                
                statusCode = HttpStatusCode.Forbidden;
                message = "You are not authorized";
            }else {
                statusCode = HttpStatusCode.InternalServerError;
                message = "Some unkown error occured.";
            }
            if(env.IsDevelopment())
            {
                response = new ApiError((int)statusCode, ex.Message, ex.StackTrace);
            } else {
                response = new ApiError((int)statusCode, message);
            }
            context.Response.StatusCode = (int)statusCode;
            logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(response.ToString());
        }
    }


}
}