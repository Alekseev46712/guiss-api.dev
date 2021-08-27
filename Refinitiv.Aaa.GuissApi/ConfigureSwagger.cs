using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Refinitiv.Aaa.GuissApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Refinitiv.Aaa.GuissApi
{
    /// <summary>
    /// Configuration of Swagger.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ConfigureSwagger
    {
        /// <summary>
        /// Configure the API to support swagger documentation.
        /// </summary>
        /// <param name="services">Service collection to add swagger services to.</param>
        /// <param name="configuration">Application configuration.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection ConfigureSwaggerServices(this IServiceCollection services, IConfiguration configuration)
        {
            var swagger = services.BuildServiceProvider().GetRequiredService<IOptions<SwaggerConfiguration>>().Value;
            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(
                        swagger.Version,
                        new OpenApiInfo
                        {
                            Title = swagger.Title,
                            Version = swagger.Version,
                            Description = swagger.Description,
                        });


                    c.OperationFilter<OptionalRouteParameterOperationFilter>();
                    c.EnableAnnotations();

                    AddSwaggerXmlComments(c);
                });

            return services;
        }

        /// <summary>
        /// Adds the swagger UI to the application.
        /// </summary>
        /// <param name="app">Application to add swagger UI to.</param>
        /// <param name="configuration">Application configuration.</param>
        /// <returns>IApplicationBuilder.</returns>
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IConfiguration configuration)
        {
            var swagger = app.ApplicationServices.GetService<IOptions<SwaggerConfiguration>>().Value; //NOSONAR
            app.UseSwagger()
               .UseSwaggerUI(
                   c =>
                   {
                       c.SwaggerEndpoint(swagger.Endpoint, swagger.Title);
                   });

            return app;
        }

        /// <summary>
        /// Adds Swagger documentation from the generated xml documentation.
        /// </summary>
        /// <param name="options">Swagger Generation options.</param>
        private static void AddSwaggerXmlComments(SwaggerGenOptions options)
        {
            foreach (var xmlDocFile in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.XML"))
            {
                options.IncludeXmlComments(xmlDocFile);
            }
        }
    }
}
