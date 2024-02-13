using rss_base.InfraStructure;
using rss_base.Models;

namespace rss_base.Services
{
    public class SessionManager: ISessionManager
    {
        private readonly ILogger<SessionManager> _logger;
        private readonly ISessionCache _sessionCache;

        public SessionManager(ILogger<SessionManager> logger, ISessionCache sessionCache)
        {
            _logger = logger;
            _sessionCache = sessionCache;
        }

        public Guid? CreateSession(string userid, string ipaddress)
        {
            var session = new Session
            {
                SessionId =  System.Guid.NewGuid(),
                UserId = userid,
                IpAddress = ipaddress,
                AttendedAllowed = true,
                UnAttendedAllowed = false,
                sessionStatus = SessionStatus.New
            };

            var result = _sessionCache.AddOrUpdateSession(session);
            if (result != null) 
            {
                return result.SessionId;
            }
            return null;
        }

        public Session? GetSessionAllow(Guid id)
        {
            return _sessionCache.GetSession(id);
        }

        public SessionStatus? GetSessionStatus(Guid id)
        {
            var session = _sessionCache.GetSession(id);
            if (session != null) 
            {
                return session.sessionStatus;
            }
            return null;
        }

        public bool SetSessionAllow(Guid id, bool attended, bool unattended)
        {
            var session = _sessionCache.GetSession(id);
            if (session == null) 
            {
                return false;
            }
            session.AttendedAllowed = attended;
            session.UnAttendedAllowed = unattended;
            var result = _sessionCache.AddOrUpdateSession(session);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public bool SetSessionStatus(Guid id, SessionStatus status)
        {
            var session = _sessionCache.GetSession(id);
            if (session == null)
            {
                return false;
            }
            session.sessionStatus = status;
            var result = _sessionCache.AddOrUpdateSession(session);
            if (result != null)
            {
                return true;
            }
            return false;
        }
    }
}
