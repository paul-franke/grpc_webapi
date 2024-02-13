//using Grpc.Core;
//using GrpcSessionManager;

//namespace GrpcServices
//{
//    public class SessionManagerGrpcClient : SessionManagerGrpcClientBase
//    {
//        private readonly ILogger<SessionManagerGrpcClient> _logger;
//        public SessionManagerGrpcClient(ILogger<SessionManagerGrpcClient> logger)
//        {
//            _logger = logger;
//        }

//        public override Task<Result> NotifySessionStatus(SessionStatusData request, ServerCallContext context)
//        {
//            //return Task.FromResult(new HelloReply
//            //{
//            //    Message = "Hello " + request.Name
//            //});
//        }
//    }
//}
