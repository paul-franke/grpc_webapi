using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Rest_Client.InfraStructure;
using Rest_Client.Configuration;
using rss_base.Controllers.Models;
using System.Text;

namespace Rest_Client.RestClient
{
    public class RestClient : BackgroundService
    {
        private readonly ILogger<RestClient> _logger;
        private readonly RestConfigurationOptions _options;
        private readonly IRestHttpClientFactory _httpClientFactory;

        public RestClient(ILogger<RestClient> logger, IOptions<RestConfigurationOptions> options, IRestHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SessionModel sessionModel = new SessionModel();
            string ?sessionId = null; 
            if (string.IsNullOrEmpty(_options.url)) 
            {
                _logger.LogError($"No url specified: exiting!  -- {DateTime.Now}");
                return;
            }

            var client = _httpClientFactory.Create();

            if (client == null) 
            {
                _logger.LogError($"Cannot create HttpClient: exiting  background process -- {DateTime.Now}");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(10000);
                    Guid tmp;

                    if (_options.CertificatePinning)
                    {
                        _logger.LogInformation($"_options.CertificatePinning true {_options.CertificatePinning}  -- {DateTime.Now}");
                    }
                    else
                    {
                        _logger.LogInformation($"_options.CertificatePinning false {_options.CertificatePinning}  -- {DateTime.Now}");
                    }
                    
                    if (!Guid.TryParse(sessionModel.SessionId, out tmp))
                    {
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/v1/rss/sessions") { Version = new Version(2, 0) };
                        request.Content = new StringContent("{  \"ipAddress\":\"192.168.2.7\"," +
                                                                "\"userId\":\"33\"}",
                                                            Encoding.UTF8,
                                                            "application/json");
                        var response = await client.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            var content = response.Content.ReadAsStringAsync().Result;
                            sessionModel = JsonConvert.DeserializeObject<SessionModel>(content);
                            _logger.LogInformation($"SetSession: session created {sessionModel.SessionId}  -- {DateTime.Now}");
                        }
                        else
                        {
                            _logger.LogError($"SetSession: Error while creating Session  -- {DateTime.Now}");
                        };
                    }

                    if (Guid.TryParse(sessionModel.SessionId, out tmp))
                    {
                        sessionId = sessionModel.SessionId ?? "";
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/rss/GetSessionStatus?Id={sessionId}") { Version = new Version(2, 0) };
 
                        var response = await client.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            var sessionStatus = response.Content.ReadAsStringAsync().Result;
                            _logger.LogInformation($"SetSessionStatus: {sessionStatus}  -- {DateTime.Now}");
                        }
                        else
                        {
                            _logger.LogError($"SetSessionStatus: Error  -- {DateTime.Now}");
                        };
                    }
                }

                catch (Exception ex)
                {
                    _logger.LogError($"SetSessionStatusAsync: Exception {ex}  -- {DateTime.Now}");
                }
            }
        }
    }
}

