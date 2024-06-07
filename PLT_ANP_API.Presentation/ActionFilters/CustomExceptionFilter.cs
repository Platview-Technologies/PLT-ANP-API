using Entities.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace PLT_ANP_API.Presentation.ActionFilters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilter> _logger;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            // Log the exception
            

            // Customize the response based on the exception type
            //var response = context.HttpContext.Response;
            

            // Delete a specific cookie based on the exception
            if (context.Exception is RefreshTokenBadRequest)
            {
                DeleteCookie(context, "rt");
            }
            
            // Set the exception as handled
            context.ExceptionHandled = true;
        }

        private void DeleteCookie(ExceptionContext context, string cookieName)
        {
            if (context.HttpContext.Request.Cookies.ContainsKey(cookieName))
            {
                context.HttpContext.Response.Cookies.Delete(cookieName);
            }
        }
    }
}


