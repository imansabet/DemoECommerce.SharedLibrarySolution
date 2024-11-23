using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace eCommerce.SharedLibrary.MiddleWare;

public class GlobalException(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context) 
    {
        //Declate variables
        string message = "Sorry,internal Server Error Occured , Kindly Try Again";
        int statusCode = (int)HttpStatusCode.InternalServerError;
        string title = "Error";

        try
        {
            await next(context);
            // check if Response is too many request : 429
            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests) 
            {
                title = "Warning";
                message = "Too Many Request Made";
                statusCode = (int)StatusCodes.Status429TooManyRequests;
                await ModifyHeader(context, title, message, statusCode);
                    
            }
            // check if Response is UnAuthorize : 401
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                title = "Alert";
                message = "You're not Authorized To Access ."; 
                statusCode = (int)StatusCodes.Status401Unauthorized;
                await ModifyHeader(context, title, message, statusCode);
            }
            // check if Response is forbidden : 403

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                title = "Out Of Access";
                message = "Your Not Allowed To Access.";
                statusCode = (int)StatusCodes.Status403Forbidden;
                await ModifyHeader(context, title, message, statusCode);
            }
        }
        catch (Exception ex) 
        {
            // Log Original Exception /File,debugger, Console
            LogException.LogExceptions(ex);

            // check if Exceptyion is Timeout : 408

            if (ex is TaskCanceledException || ex is TimeoutException) 
            {
                title = "Out Of Time";
                message = "Request Time Out .. Try Again";
                statusCode = StatusCodes.Status408RequestTimeout;
            }
            //default:
            await ModifyHeader(context, title, message, statusCode);
        }
    }

    private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
        {
            Detail = message,
            Status = statusCode,
            Title = title
        }),CancellationToken.None) ;
        return;
    }
}
