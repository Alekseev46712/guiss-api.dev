using System.Diagnostics.CodeAnalysis;
using Amazon.Lambda.AspNetCoreServer;
using Microsoft.AspNetCore.Hosting;

namespace Refinitiv.Aaa.GuissApi
{
    /// <summary>
    /// Entry point for running the Guiss API as an AWS Lambda function.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        /// <inheritdoc />
        protected override void Init(IWebHostBuilder builder)
        {
            builder.UseStartup<Startup>();
        }
    }
}
