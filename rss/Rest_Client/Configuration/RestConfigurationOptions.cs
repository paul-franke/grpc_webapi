namespace Rest_Client.Configuration
{
    public class RestConfigurationOptions
    {
        public string? LeafCertificateThumbString { get; set; }
        public string? IntermediateCertificateThumbString { get; set; }
        public bool CertificatePinning { get; set; }
        public string? url { get; set; }
        public int RestClientTimeOut {  get; set; }
    }
}
