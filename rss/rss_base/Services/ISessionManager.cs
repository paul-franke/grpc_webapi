using rss_base.Models;

namespace rss_base.Services
{
    public interface ISessionManager
    {
        public bool SetSessionStatus(Guid id, SessionStatus status);
        public bool SetSessionAllow(Guid id, bool attended, bool unattended);
        public SessionStatus? GetSessionStatus(Guid id);
        public Session? GetSessionAllow(Guid id);
        public Guid? CreateSession(string userid, string ipaddress);
    }
}
