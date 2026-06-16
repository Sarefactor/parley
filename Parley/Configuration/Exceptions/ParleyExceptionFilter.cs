using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Parley.Configuration.Exceptions;

public sealed class ParleyExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is not ParleyValidationException ex)
        {
            return;
        }

        context.Result = new UnprocessableEntityObjectResult(ex.Context);
        context.ExceptionHandled = true;
    }
}