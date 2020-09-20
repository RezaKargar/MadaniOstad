using KodoomOstad.Common.Exceptions;
using KodoomOstad.IocConfig.Api;
using KodoomOstad.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace KodoomOstad.IocConfig.Filters
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var apiResult = new ApiResult();

            if (context.Result is BadRequestObjectResult badRequestObjectResult)
            {
                var errorsList = new List<string>();

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

                apiResult = new ApiResult(errors: errorsList.ToArray());

            }
            else if (context.Result is NotFoundObjectResult notFoundObjectResult)
            {
                apiResult = new ApiResult(CastToStringArray(notFoundObjectResult.Value));
            }
            else if (context.Result is ObjectResult objectResult && objectResult.StatusCode == 404)
            {
                context.Result = new NotFoundResult();
            }
            else if (context.Result is ForbidResult)
            {
                apiResult = new ApiResult("You don't have permission to access.");
            }
            else if (context.Result is OkObjectResult okObjectResult)
            {
                if (!(okObjectResult.Value is AccessToken))
                    apiResult = new ApiResult(okObjectResult.Value);
            }
            else if (context.Result is UnauthorizedObjectResult unauthorizedObjectResult)
            {
                apiResult = new ApiResult(CastToStringArray(unauthorizedObjectResult.Value));
            }

            var isContextResultChanged = apiResult.Data != null || apiResult.Errors.Any();
            if (isContextResultChanged)
                context.Result = new JsonResult(apiResult);

            base.OnResultExecuting(context);
        }

        private static string[] CastToStringArray(object value)
        {
            switch (value)
            {
                case string valueAsString:
                    return new[] { valueAsString };
                case string[] valueAsStringArray:
                    return valueAsStringArray;
                case List<string> list:
                    return list.ToArray();
                default:
                    throw new AppException("Can't cast non-string or non-string array to string array", HttpStatusCode.InternalServerError);
            }
        }
    }
}
