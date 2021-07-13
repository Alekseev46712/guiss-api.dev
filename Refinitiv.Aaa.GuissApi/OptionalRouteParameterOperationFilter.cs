using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Refinitiv.Aaa.GuissApi
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class OptionalRouteParameterOperationFilter : IOperationFilter
    {
        private const string CaptureName = "searchCriteria";

        /// <inheritdoc />
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var httpMethodAttributes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute>();

            var httpMethodWithOptional = httpMethodAttributes.Where(a => a.Template != null).FirstOrDefault(m => m.Template.Contains("?", StringComparison.InvariantCulture));
            if (httpMethodWithOptional == null)
            {
                return;
            }

            var regex = $"{{(?<{CaptureName}>\\w+)\\?}}";

            var matches = System.Text.RegularExpressions.Regex.Matches(httpMethodWithOptional.Template, regex);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var name = match.Groups[CaptureName].Value;

                var parameter = operation.Parameters.FirstOrDefault(p => p.In == ParameterLocation.Path && p.Name == name);
                if (parameter == null)
                {
                    continue;
                }

                parameter.AllowEmptyValue = true;
                parameter.Description = "Must check \"Send empty value\" or Swagger passes a comma for empty values otherwise";
                parameter.Required = false;
                parameter.Schema.Nullable = true;
            }
        }
    }
}
