using Grpc.Net.Client;

namespace rss_base.InfraStructure
{
    public interface IGrpcClientChannel
    {
        public GrpcChannel Channel 
        {
            get;   
        }

    }
}
