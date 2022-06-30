using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MNP.DIH.Shared.Cosmos;

// Set up and run host:
using var host = new HostBuilder()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureServices(services =>
	{
		services.AddApplicationInsightsTelemetryWorkerService(opts => opts.DependencyCollectionOptions.EnableLegacyCorrelationHeadersInjection = true)
			.AddCosmosContainerHandle(
				cosmosEndpoint: "https://scheduling-dbacct-dev.documents.azure.com:443/",
				defaultDbName: "scheduling-dbacct-dev-db",
				defaultContainerName: "PersonnelEngagementSchedule");
	})
	.Build();
await host.RunAsync();


[ExcludeFromCodeCoverage]
#pragma warning disable S1118 // Utility classes should not have public constructors
#pragma warning disable S3903 // Types should be defined in named namespaces
internal partial class Program { }
#pragma warning restore S3903 // Types should be defined in named namespaces
#pragma warning restore S1118 // Utility classes should not have public constructors