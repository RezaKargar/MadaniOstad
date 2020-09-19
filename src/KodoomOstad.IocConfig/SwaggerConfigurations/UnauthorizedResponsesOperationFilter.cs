using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KodoomOstad.IocConfig.SwaggerConfigurations
{
    public class UnauthorizedResponsesOperationFilter : IOperationFilter
    {
        private readonly bool _includeUnauthorizedAndForbiddenResponses;
        private readonly string _schemeName;

        public UnauthorizedResponsesOperationFilter(bool includeUnauthorizedAndForbiddenResponses, string schemeName = "Bearer")
        {
            this._includeUnauthorizedAndForbiddenResponses = includeUnauthorizedAndForbiddenResponses;
            this._schemeName = schemeName;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filters = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;

            var hasAnonymous = filters.Any(p => p.Filter is AllowAnonymousFilter) || metadata.Any(p => p is AllowAnonymousAttribute);
            if (hasAnonymous) return;

            var hasAuthorize = filters.Any(p => p.Filter is AuthorizeFilter) || metadata.Any(p => p is AuthorizeAttribute);
            if (!hasAuthorize) return;

            if (_includeUnauthorizedAndForbiddenResponses)
            {
                var unauthorizedApiMediaTypes = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        "application/json", new OpenApiMediaType
                        {
                            Example = new OpenApiString(
                                "{\n  \"errors\": [\n    \"Authentication needed.\"\n    ]\n}"
                            )
                        }
                    }
                };

                var forbiddenApiMediaTypes = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        "application/json", new OpenApiMediaType
                        {
                            Example = new OpenApiString(
                                "{\n  \"errors\": [\n    \"You don't have permission to access.\"\n    ]\n}"
                            )
                        }
                    }
                };


                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized", Content = unauthorizedApiMediaTypes });
                operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden", Content = forbiddenApiMediaTypes });
            }

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Scheme = _schemeName,
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OAuth2" }
                    },
                    Array.Empty<string>()
                }
            });
        }
    }
}
