namespace Rest_Client.InfraStructure
{
    public interface IRestHttpClientFactory
    {
        public HttpClient? Create();
    }
}
