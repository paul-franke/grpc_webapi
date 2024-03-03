namespace Grpc_Client.Configuration
{
    public class GrpcConfigurationOptions
    {

        public string? LeafCertificateThumbString { get; set; }
        public string? IntermediateCertificateThumbString { get; set; }
        public bool CertificatePinning { get; set; }
        public string? url { get; set; }
        public int GrpcClientTimeOut {  get; set; }
    }
}
