using Microsoft.EntityFrameworkCore;
using SmartPark.Common.Wrapper;
using SmartPark.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace SmartPark.Middlwares
{
    public class GlobleExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly IServiceScopeFactory _scopeFactory;

        public GlobleExceptionHandlerMiddleware(RequestDelegate requestDelegate, IServiceScopeFactory scopeFactory)
        {
            _requestDelegate = requestDelegate;
            _scopeFactory = scopeFactory;
        }


        public async Task Invoke(HttpContext httpcontext)
        {
            try
            {
                await _requestDelegate(httpcontext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpcontext, ex);
            }
            // Check for 401 Unauthorized response
            if (httpcontext.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                var problemDetail = new ErrorDetails
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Unauthorized access.",
                    Path = httpcontext.Request.Path
                };

                httpcontext.Response.ContentType = "application/json";
                await httpcontext.Response.WriteAsync(JsonSerializer.Serialize(problemDetail));
            }
            // Check for 403 Forbidden response
            if (httpcontext.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                var problemDetail = new ErrorDetails
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "You do not have permission to access this resource.",
                    Path = httpcontext.Request.Path
                };

                httpcontext.Response.ContentType = "application/json";
                await httpcontext.Response.WriteAsync(JsonSerializer.Serialize(problemDetail));
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            var response = httpContext.Response;
            response.ContentType = "application/json";

            var statusCode = exception switch
            {
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                UnAuthorizeException => StatusCodes.Status401Unauthorized,
                BadHttpRequestException => StatusCodes.Status400BadRequest,
                ValidationException => StatusCodes.Status400BadRequest,
                ForBiddenException => StatusCodes.Status403Forbidden,
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                DbUpdateException => StatusCodes.Status500InternalServerError,
                ArgumentNullException => StatusCodes.Status400BadRequest,
                ArgumentException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                TimeoutException => StatusCodes.Status504GatewayTimeout,
                TaskCanceledException => StatusCodes.Status408RequestTimeout,
                JsonException => StatusCodes.Status400BadRequest,
                //AutoMapperMappingException => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError,
            };
            string message = exception.Message; // Default
            if (exception is DbUpdateException dbEx)
            {
                var errorMessage = dbEx.InnerException?.InnerException?.Message ?? dbEx.InnerException?.Message ?? dbEx.Message;

                if (!string.IsNullOrEmpty(errorMessage) &&
                    errorMessage.IndexOf("foreign key", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    message = "Invalid reference detected. Please ensure the related data exists.";

                }
                else if (!string.IsNullOrEmpty(errorMessage) && errorMessage.IndexOf("REFERENCE", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    message = "Unable to delete this record because it is linked to other data. Please remove related records first.";

                }
                else if (errorMessage.IndexOf("UNIQUE", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Example: duplicate record insertion
                    message = "A record with similar details already exists.";
                }
                else
                {
                    message = "A database update error occurred.";
                }
            }
            response.StatusCode = statusCode;

            var problemDetail = new ErrorDetails
            {
                StatusCode = statusCode,
                Message = message,
                Path = httpContext.Request.Path
            };
            //using (var scop = _scopeFactory.CreateScope())
            //{
                //var helper = scop.ServiceProvider.GetRequiredService<IHelper>();
                //var UOW = scop.ServiceProvider.GetRequiredService<IUnitOfWork>();
                //var userId = await helper.GetUserIdFromToken();
                //var storeException = new TblException
                //{
                //    UserId = userId != 0 ? userId : (int?)null,
                //    Message = exception.Message,
                //    StatusCode = statusCode,
                //    Path = httpContext.Request.Path,
                //    CreatedOn = DateTime.Now
                //};
                //// Store only critical exceptions in the database
                //if (statusCode == StatusCodes.Status500InternalServerError)
                //{
                //    try
                //    {
                //        await UOW.ExceptionRepo.AddException(storeException);
                //        await UOW.SaveChangesAsync();
                //    }
                //    catch (Exception ex)
                //    {

                //        var errorDetails = new ErrorDetails
                //        {
                //            StatusCode = StatusCodes.Status500InternalServerError,
                //            Message = "An error occurred while logging the first exception: " + ex.InnerException.Message,
                //            Path = httpContext.Request.Path
                //        };

                //        httpContext.Response.ContentType = "application/json";
                //        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(errorDetails));
                //        return;
                //    }

                //}
            //}
            await response.WriteAsync(JsonSerializer.Serialize(problemDetail));
        }

    }
}
