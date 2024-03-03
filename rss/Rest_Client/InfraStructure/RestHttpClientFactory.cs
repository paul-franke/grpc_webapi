using Microsoft.Extensions.Options;
using Rest_Client.Configuration;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Channels;

namespace Rest_Client.InfraStructure
{
    public class RestHttpClientFactory: IRestHttpClientFactory
    {
        private readonly ILogger<RestHttpClientFactory>? _logger;
        private readonly RestConfigurationOptions? _options;
        private static string? _intermediateCertifcate;
        private static string? _leafCertifcate;

        public RestHttpClientFactory(ILogger<RestHttpClientFactory> logger, IOptions<RestConfigurationOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _intermediateCertifcate = options?.Value.IntermediateCertificateThumbString;
            _leafCertifcate = options?.Value.LeafCertificateThumbString;
        }

        public RestHttpClientFactory() { }
        public HttpClient? Create()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            if (_options != null && _options.CertificatePinning)
            {
                handler.ServerCertificateCustomValidationCallback =
                    (_, cert, certChain, policyErrors) =>
                        {
                            _logger?.LogInformation($"cert?.Thumbprint {cert?.Thumbprint}  -- {DateTime.Now}");
                            if (String.Equals(cert?.Thumbprint, _leafCertifcate, StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
                            return (certChain != null && certChain.ChainElements.Any<X509ChainElement>(x =>
                                    String.Equals(x.Certificate.Thumbprint, _intermediateCertifcate, StringComparison.OrdinalIgnoreCase)));
                        };
            }
            var client = new HttpClient(handler);

            if (string.IsNullOrEmpty(_options?.url))
            {
                _logger?.LogError($"No url specified: Assuming https://localhost:443!  -- {DateTime.Now}");
            }
            client.BaseAddress = new Uri(_options?.url ?? "https://localhost:443");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}