using Grpc.Net.Client;
using GrpcSessionClient;
using Microsoft.Extensions.Options;
using Grpc_Client.Configuration;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace rss_base.InfraStructure
{
    public class GrpcClientChannel : IGrpcClientChannel
    {
        private readonly GrpcChannel _channel;
        private static string? intermediateCertifcate;
        private static string? leafCertifcate;

        public GrpcChannel Channel   // property
        {
            get { return _channel; }   // get method
        }

        public GrpcClientChannel(IOptions<GrpcConfigurationOptions> options, ILogger<SessionManagerClientBackGround> logger) 
        {
            intermediateCertifcate = options?.Value.IntermediateCertificateThumbString;
            leafCertifcate = options?.Value.LeafCertificateThumbString; 
            var handler = new HttpClientHandler();

            if (options!=null && options.Value.CertificatePinning) 
            {
                handler.ServerCertificateCustomValidationCallback = ServerCertificateCustomValidation;
            }

            if (string.IsNullOrEmpty(options.Value.url))
            {
                logger.LogError($"No url specified: Assuming https://localhost:443!  -- {DateTime.Now}");
            }
            _channel = GrpcChannel.ForAddress(options?.Value.url??"https://localhost:443",
                new GrpcChannelOptions { HttpHandler = handler });
        }

        private static bool ServerCertificateCustomValidation(HttpRequestMessage requestMessage, X509Certificate2? certificate, X509Chain? chain, SslPolicyErrors sslErrors)
        {
            return String.Equals(certificate?.Thumbprint, intermediateCertifcate, StringComparison.OrdinalIgnoreCase) ||
                   String.Equals(certificate?.Thumbprint, leafCertifcate, StringComparison.OrdinalIgnoreCase);
        }
    }
}
