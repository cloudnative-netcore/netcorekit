using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NetCoreKit.Infrastructure.AspNetCore.OpenApi
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            // Policy names map to scopes
            /*var requiredScopes = context.MethodInfo
              .GetCustomAttributes(true)
              .OfType<AuthorizeAttribute>()
              .Select(attr => attr.Policy)
              .Distinct();*/

            var requiredScopes = context.MethodInfo.DeclaringType
                ?.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>()
                .ToList();

            if (requiredScopes == null || !requiredScopes.Any()) return;

            operation.Responses.Add("401", new Response {Description = "Unauthorized"});
            operation.Responses.Add("403", new Response {Description = "Forbidden"});

            operation.Security = new List<IDictionary<string, IEnumerable<string>>>
            {
                new Dictionary<string, IEnumerable<string>>
                {
                    ["oauth2"] = requiredScopes.Select(attr => attr.Policy).Distinct()
                }
            };
        }
    }
}
