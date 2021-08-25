using System.Diagnostics.CodeAnalysis;
using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using Refinitiv.Aaa.Api.Common.Extensions;
using Refinitiv.Aaa.Api.Common.Middleware;
using Refinitiv.Aaa.GuissApi.Facade;
using Refinitiv.Aaa.Logging;
using Refinitiv.Aaa.Logging.Interfaces;
using Refinitiv.Aaa.Pagination;
using Refinitiv.Aaa.GuissApi.Facade.Extensions;
using Refinitiv.Aaa.GuissApi.Models;
using Refinitiv.Aaa.Interfaces.Headers;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;
using Refinitiv.Aaa.Foundation.ApiClient.Constants;
using System;
using Refinitiv.Aaa.Foundation.ApiClient.Core.Models;
using Refinitiv.Aaa.GuissApi.Middlewares;
using Refinitiv.Aaa.Foundation.ApiClient.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Models;
using Enyim.Caching.Configuration;
using Enyim.Caching;

namespace Refinitiv.Aaa.GuissApi
{
    /// <summary>
    /// The start up class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;
        private readonly MemcachedClientConfiguration clientConfiguration = new MemcachedClientConfiguration();
        private const string SwaggerSection = "Swagger";
        private const string LoggingSection = "Logging";
        private const string AppSettingsSection = "AppSettings";
        private const string CacheSection = "AppSettings:Cache";
        private const string ParameterStoreSection = "ParameterStore";
        private const string ParameterStoreCacheSection = "ParameterStore:CacheSettings";
        private const string ElasticacheSection = "AppSettings:Elasticache";
        private const string PaginationStoreHashPath = "ParameterStore:PaginationParameterStorePath";
        private const string UserApiBaseAddress = "AppSettings:Services:UserApi";
        private const string ElasticacheServerAddress = "AppSettings:Elasticache:Hostname";
        private const string ElasticacheServerPort = "AppSettings:Elasticache:Port";    

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration parameter.</param>
        /// <param name="environment">Hosting environment.</param>
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.environment = environment;
        }

        /// <summary>
        /// Method to Configure services.
        /// </summary>
        /// <param name="services">IServiceCollection parameter.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddMvc()
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter());
                });
            services.AddSwaggerGenNewtonsoftSupport();
            services.UseAaaRequestHeaders();

            services.Configure<CacheHelperOptions>(configuration.GetSection(ElasticacheSection));          
            clientConfiguration.AddServer(configuration.GetValue<string>(ElasticacheServerAddress), configuration.GetValue<int>(ElasticacheServerPort));
            services.AddScoped<IMemcachedClient, MemcachedClient>(x => new MemcachedClient(clientConfiguration));
            services.AddScoped<IMemcachedResultsClient, MemcachedClient>(x => new MemcachedClient(clientConfiguration));
            services.AddScoped<ICacheHelper, CacheHelper>(); 

            services.Configure<SwaggerConfiguration>(configuration.GetSection(SwaggerSection));
            services.Configure<LoggingConfiguration>(configuration.GetSection(LoggingSection));
            services.Configure<AppSettingsConfig>(configuration.GetSection(AppSettingsSection));
            services.Configure<ParameterStoreConfig>(configuration.GetSection(ParameterStoreSection));
            services.Configure<CachingOptions>(configuration.GetSection(CacheSection));

            services.Configure<Ciam.SharedLibrary.Services.Models.CachingOptions>(configuration.GetSection(ParameterStoreCacheSection));

            services.AddControllers();
            services.AddRouting();

            services
                .AddHttpContextAccessor()
                .ConfigureFacade(configuration)
                .ConfigureSwaggerServices(configuration);

            services.AddHttpClient(ServiceNames.UserApi, c =>
            {
                c.BaseAddress = new Uri(configuration[UserApiBaseAddress]);
            }).AddHttpMessageHandler<ErrorHandlingDelegatingHandler>();

            if (environment.IsDevelopment())
            {
                services.UseRefinitivPagination();
                services.AddLogging(loggingBuilder => loggingBuilder.AddDebug());
            }
            else
            {
                services.UseRefinitivPagination(configuration[PaginationStoreHashPath]);
            }
        }

        /// <summary>
        /// Method to Configure application.
        /// </summary>
        /// <param name="app">IApplicationBuilder parameter.</param>
        /// <param name="env">IHostingEnvironment parameter.</param>
        /// <remarks>This method gets called by the runtime.</remarks>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCorrelationId();

            app
                .UseMiddleware<GuissExceptionHandlerMiddleware>()
                .UseMiddleware<ExceptionHandlerMiddleware>()
                .UseMiddleware<RequestLoggingMiddleware>();

            var logger = app.ApplicationServices.GetService<IOptions<LoggingConfiguration>>().Value;
            // Configure the refinitiv logger
            if (logger.Enabled)
            {
                app.UseRefinitivLogger(
                    c =>
                    {
                        c.SourceContext = logger.Context;
                        c.Category = logger.Category;
                        c.IdentityPoolId = logger.IdentityPoolId;
                        c.Target = logger.Target.ToEnum<LoggingTarget>();
                        c.LoggingEndpoint = logger.Endpoint;
                        c.CorrelationId = () =>
                        {
                            var serviceScopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
                            using var scope = serviceScopeFactory.CreateScope();
                            var headers = scope.ServiceProvider.GetService<IAaaRequestHeaders>();
                            return headers?.CorrelationId;
                        };
                    });
            }

            app.UseRouting();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(configuration);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
