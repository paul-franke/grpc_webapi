using Rest_Client.InfraStructure;
using Rest_Client.Configuration;

namespace Rest_Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IHostedService, Rest_Client.RestClient.RestClient>();
                services.AddSingleton<IRestHttpClientFactory, RestHttpClientFactory>();
                services.AddOptions<RestConfigurationOptions>().BindConfiguration("RestClientConfigurationOptions");

            }).Build();
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

            await host.RunAsync();
        }
    }
}

 
 