using Grpc.Core;
using rss_base.Models;
using rss_base.Services;

namespace GrpcSessionManager
{
    public class SessionManagerGrpcServer : SessionManagerServerDefinition.SessionManagerServerDefinitionBase
    {
        private readonly ILogger<SessionManagerGrpcServer> _logger;
        private readonly ISessionManager _sessionManager;

        public SessionManagerGrpcServer(ILogger<SessionManagerGrpcServer> logger, ISessionManager sessionManager)
        {
            _logger = logger;
            _sessionManager = sessionManager;
        }

        public override async Task<ResultCode> SetSessionStatus(SessionStatusData request, ServerCallContext context)
        {
            int rc = 0;
            _logger.LogInformation($"Calling SetSessionStatus.");

            if (!Guid.TryParse(request.Guid, out _))
            {
                _logger.LogWarning($"SetSessionStatus called with invalid Guid/SessionId.");
                rc = 1;
            }
            else
            {
                var result = _sessionManager.SetSessionStatus(Guid.Parse(request.Guid), (SessionStatus)request.Status);
                if (!result)
                {
                    _logger.LogWarning($"SetSessionStatus called with unknown Guid/SessionId.");
                    rc = 1;
                }
            }
            return await Task.FromResult(new ResultCode
            {
                CallResult = rc
            }) ;  
        }

        public override Task<ResultCode> SetSessionAllow(SessionAllowData request, ServerCallContext context)
        {
            int rc = 0;
            _logger.LogInformation($"Calling SetSessionAllow.");

            if (!Guid.TryParse(request.Guid, out _))
            {
                _logger.LogWarning($"SetSessionAllow called with invalid Guid/SessionId.");
                rc = 1;
            }
            else
            {
                var result = _sessionManager.SetSessionAllow(Guid.Parse(request.Guid), request.Attended, request.UnAttended);

                if (!result)
                {
                    _logger.LogWarning($"SetSessionAllow called with unknown Guid/SessionId.");
                    rc = 1;
                }
            }
            return Task.FromResult(new ResultCode
            {
                CallResult = rc
            });
        }

        public override Task<SessionStatusData> GetSessionStatus(SessionId request, ServerCallContext context)
        {
            _logger.LogInformation($"Calling GetSessionStatus.");

            if (!Guid.TryParse(request.Guid, out _))
            {
                _logger.LogWarning($"GetSessionStatus called with invalid Guid/SessionId.");
            }
            else
            {
                var result = _sessionManager.GetSessionStatus(Guid.Parse(request.Guid));
                if (result != null)
                {
                    return Task.FromResult(new SessionStatusData
                    {
                        Status = (int)result,
                        Guid = request.Guid
                    });
                }
                _logger.LogWarning($"GetSessionStatus called with unknown Guid/SessionId.");
            }

            return Task.FromResult(new SessionStatusData
            {
                Status = 1,
                Guid = ""
            });
        }

        public override Task<SessionAllowData> GetSessionAllow(SessionId request, ServerCallContext context)
        {
            _logger.LogInformation($"Calling GetSessionAllow.");

            if (!Guid.TryParse(request.Guid, out _))
            {
                _logger.LogWarning($"GetSessionAllow called with invalid Guid/SessionId.");
            }
            else
            {
                var result = _sessionManager.GetSessionAllow(Guid.Parse(request.Guid));
                if (result != null)
                {
                    return Task.FromResult(new SessionAllowData
                    {
                        Attended = result.AttendedAllowed,
                        UnAttended = result.UnAttendedAllowed,
                        Guid = result.SessionId.ToString()
                    });
                }
                _logger.LogWarning($"GetSessionAllow called with unknown Guid/SessionId.");
            }

            return Task.FromResult(new SessionAllowData
            {
                Attended = true,
                UnAttended = true,
                Guid = ""
            });
        }
    }
}

