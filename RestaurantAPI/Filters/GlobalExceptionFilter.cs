using Domain.Exceptions;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RestaurantAPI.Filters
{
    public class GlobalExceptionFilter: IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is DomainException)
            {
                context.Result = new BadRequestObjectResult(new { error = context.Exception.Message });
                context.ExceptionHandled = true;
            }
            else if (context.Exception is ApplicationException)
            {
                context.Result = new UnprocessableEntityObjectResult(new { error = context.Exception.Message });
                context.ExceptionHandled = true;
            }
            else if (context.Exception is InfrastructureException)
            {
                context.Result = new StatusCodeResult(500);
                context.ExceptionHandled = true;
            }
            // Add more exception types as needed
        }
    }
}
