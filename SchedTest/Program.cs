using System.Diagnostics.CodeAnalysis;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Set up and run host:
using var host = new HostBuilder()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureServices(services =>
	{
		services
			.AddApplicationInsightsTelemetryWorkerService(opts => opts.DependencyCollectionOptions.EnableLegacyCorrelationHeadersInjection = true)
			.AddSingleton(_ =>
				new CosmosClient(
					"https://scheduling-dbacct-dev.documents.azure.com:443/",
					new DefaultAzureCredential(new DefaultAzureCredentialOptions()),
					new CosmosClientOptions { ConnectionMode = ConnectionMode.Gateway }));
	})
	.Build();
await host.RunAsync();


[ExcludeFromCodeCoverage]
#pragma warning disable S1118 // Utility classes should not have public constructors
#pragma warning disable S3903 // Types should be defined in named namespaces
internal partial class Program { }
#pragma warning restore S3903 // Types should be defined in named namespaces
#pragma warning restore S1118 // Utility classes should not have public constructors