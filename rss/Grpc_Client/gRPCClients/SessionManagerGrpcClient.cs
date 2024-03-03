using Microsoft.Extensions.Options;
using Grpc_Client.Configuration;
using rss_base.InfraStructure;
using static GrpcSessionClient.SessionManagerServerDefinition;

namespace GrpcSessionClient
{
    public class SessionManagerClientBackGround: BackgroundService
    {
        private readonly ILogger<SessionManagerClientBackGround> _logger;
        private readonly IGrpcClientChannel  _channel;
        private readonly GrpcConfigurationOptions _options;

        public SessionManagerClientBackGround(ILogger<SessionManagerClientBackGround> logger, IGrpcClientChannel grpcClientChannel, IOptions<GrpcConfigurationOptions> options)
        {
            _logger = logger;
            _channel = grpcClientChannel;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var client = new SessionManagerServerDefinitionClient(_channel.Channel);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var reply = await client.SetSessionStatusAsync(new SessionStatusData
                        {
                            Guid = new Guid().ToString(),
                            Status = 2
                        }, deadline: DateTime.UtcNow.AddSeconds(_options.GrpcClientTimeOut));

                        if (reply != null)
                        {
                            _logger.LogInformation($"SetSessionStatusAsync: {reply} -- {DateTime.Now}");
                        }
                        else
                        {
                            _logger.LogInformation($"SetSessionStatusAsync: null -- {DateTime.Now}");
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"SetSessionStatusAsync: Exception {ex}  -- {DateTime.Now}");
                    }
                    await Task.Delay(10000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"SetSessionStatusAsync: Exception {ex}  -- {DateTime.Now}");
                await Task.FromException(ex);
            }
        }
    }
}

