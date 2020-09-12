using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using KodoomOstad.Common.Exceptions;

namespace KodoomOstad.IocConfig.Filters
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult badRequestObjectResult && badRequestObjectResult.StatusCode == 400)
            {
                var errorsList = new List<string>();

                var a = badRequestObjectResult.Value;
                switch (badRequestObjectResult.Value)
                {
                    case ValidationProblemDetails validationProblemDetails:
                        var errorMessages =
                            validationProblemDetails.Errors.SelectMany(p => p.Value).Distinct();
                        errorsList.AddRange(errorMessages);
                        break;

                    case List<IdentityError> identityErrors:
                        errorsList.AddRange(identityErrors.Select(ie => ie.Description));
                        break;

                    case SerializableError errors:
                        var errorMessages2 =
                            errors.SelectMany(p => (string[])p.Value).Distinct();
                        errorsList.AddRange(errorMessages2);
                        break;

                    case var value when value != null && !(value is ProblemDetails):
                        errorsList.Add(badRequestObjectResult.Value.ToString());
                        break;
                }

                context.Result = new JsonResult(new
                {
                    Errors = errorsList
                });

            }
            else if (context.Result is ObjectResult notFoundObjectResult && notFoundObjectResult.StatusCode == 404)
            {
                context.Result = new NotFoundObjectResult(new
                {
                    Errors = new[] { "Not Found" }
                });
            }
            else if (context.Result is ForbidResult forbidResult)
            {
                context.Result = new NotFoundObjectResult(new
                {
                    Errors = new[] { "You don't have permission to access." }
                });
            }
            base.OnResultExecuting(context);
        }
    }
}
