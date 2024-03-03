using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using rss_base.Controllers.Models;
using rss_base.Models;
using rss_base.Services;

namespace rss_base.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/rss")]
    public class SessionManagerController : ControllerBase
    {
        private readonly ILogger<SessionManagerController> _logger;
        private readonly ISessionManager _sessionManager;

        public SessionManagerController(ILogger<SessionManagerController> logger, ISessionManager sessionManager)
        {
            _logger = logger;
            _sessionManager = sessionManager;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetSessionStatus")]
        public async Task<ActionResult> GetSessionStatus( Guid id)
        {
            _logger.LogInformation($"Calling GetSessionStatus.");
            var result = _sessionManager.GetSessionStatus(id);
            if (result != null)
            {
                return await Task.FromResult(Ok(result.ToString()));
            }
            return await Task.FromResult(NotFound());
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetSessionAllow")]
        public async Task<ActionResult<GetSessionAllow>> GetSessionAllow( Guid id)
        {
            _logger.LogInformation($"Calling GetSessionStatus.");
            var result = _sessionManager.GetSessionAllow(id);
            if (result != null)
            {
                var sessionAllow = new GetSessionAllow
                {
                    Attended = result.AttendedAllowed,
                    UnAttended = result.UnAttendedAllowed
                };
                return await Task.FromResult(Ok(sessionAllow));
            }
            return await Task.FromResult(NotFound());
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("sessions/{session_id}/sessions")]
        public async Task<ActionResult<string>> SetSessionStatus(string session_id, SetSessionStatus status)
        {
            _logger.LogInformation($"Calling SetSessionStatus.");

            if (Guid.TryParse(session_id, out _))
            {
                var result = _sessionManager.SetSessionStatus(Guid.Parse(session_id), (SessionStatus)status.SessionStatus);
                if (result)
                {
                    return await Task.FromResult(Ok());
                }
                return await Task.FromResult(NotFound(StatusCodes.Status500InternalServerError));
            }
            return await Task.FromResult(NotFound());
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("SetSessionAllow")]
        public async Task<ActionResult> SetSessionAllow(SetSessionAllow allow)
        {
            _logger.LogInformation($"Calling SetSessionAllow.");
            var result = _sessionManager.SetSessionAllow(allow.sessionId, allow.Attended, allow.UnAttended);
            // to notify status to rss

            if (result)
            {
                return await Task.FromResult(Ok());
            }
            return await Task.FromResult(NotFound());
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("sessions")]
        public async Task<ActionResult<SessionModel>> CreateSession(CreateSessionModel createSessionModel)
        {
            _logger.LogInformation($"Calling RequestSession.");
            var result = _sessionManager.CreateSession(createSessionModel.UserId??"", createSessionModel.IpAddress ?? "");
            if (result != null)
            {
                return await Task.FromResult(Ok(new SessionModel{SessionId = result.Value.ToString() }));
            }
            return await Task.FromResult(NotFound(StatusCodes.Status500InternalServerError));
        }
    }
}
