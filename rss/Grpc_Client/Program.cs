using GrpcSessionClient;
using Grpc_Client.Configuration;
using rss_base.InfraStructure;

namespace Grpc_Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IHostedService, SessionManagerClientBackGround>();
                services.AddSingleton<IGrpcClientChannel, GrpcClientChannel>();
                services.AddOptions<GrpcConfigurationOptions>().BindConfiguration("GrpcConfigurationOptions");
            }).Build();
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

            await host.RunAsync();
        }
    }
}
