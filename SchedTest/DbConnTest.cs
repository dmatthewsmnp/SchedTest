using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace SchedTest;

public class DbConnTest
{
	private readonly Container _container;
	private readonly ILogger _logger;

	public DbConnTest(CosmosClient cosmosClient, ILoggerFactory loggerFactory)
	{
		_logger = loggerFactory.CreateLogger<DbConnTest>();
		_container = cosmosClient.GetContainer("scheduling-dbacct-dev-db", "PersonnelEngagementSchedule");
	}

	[Function("DbConnTest")]
	public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dbconntest")] HttpRequestData req)
	{
		_logger.LogInformation("Starting function");
		//try
		//{
		//	await HostTest("scheduling-dbacct-dev.documents.azure.com");
		//}
		//catch (Exception ex)
		//{
		//	_logger.LogError(ex, "Error running connection test (CosmosDb)");
		//}
		//try
		//{
		//	await HostTest("schdlfncstrgedev.blob.core.windows.net");
		//}
		//catch (Exception ex)
		//{
		//	_logger.LogError(ex, "Error running connection test (storage)");
		//}

		try
		{
			var itemResponse = await _container.ReadItemAsync<SampleDoc>("12fc0713-54e0-484c-b44c-ab4c9b431066", new PartitionKey("f7ed658e-b152-48c4-9e94-407f43a8f02d"));
			var response = req.CreateResponse();
			await response.WriteAsJsonAsync(itemResponse.Resource);
			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error connecting to database");
			return req.CreateResponse(HttpStatusCode.InternalServerError);
		}
	}

	private async Task HostTest(string hostName)
	{
		// Run DNS lookup and report result:
		var ipaddr = Dns.GetHostEntry(hostName).AddressList.First();
		_logger.LogInformation("DNS results for {hostName}: {ipaddr}", hostName, ipaddr);

		// Attempt basic socket connection:
		using var tcpClient = new TcpClient();
		await tcpClient.ConnectAsync(hostName, 443);
		_logger.LogInformation("TcpClient connected successfully to {hostName}", hostName);

		// Attempt SSL negotiation on open socket:
		using var sslStream = new SslStream(
			tcpClient.GetStream(),
			true,
			new RemoteCertificateValidationCallback((object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors) => true),
			null);
		await sslStream.AuthenticateAsClientAsync(hostName);
		_logger.LogInformation("SslStream connected successfully to {hostName} with {algorithm}", hostName, sslStream.CipherAlgorithm);
	}
}

public class SampleDoc
{
	public Guid id { get; set; }
}
