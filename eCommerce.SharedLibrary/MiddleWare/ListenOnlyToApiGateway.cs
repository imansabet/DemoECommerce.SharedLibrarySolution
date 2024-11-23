using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.MiddleWare;

public class ListenOnlyToApiGateway(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context) 
    {
        // Extract Specific Header From Request
        var signedHeader = context.Request.Headers["Api-gateway"];

        // null ? Request not coming from Gateway
        if (signedHeader.FirstOrDefault() is null)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync("Sorry , Service Is Unavailable;");
            return;
        }
        else 
        {
            await next(context);
        }

    } 
}
