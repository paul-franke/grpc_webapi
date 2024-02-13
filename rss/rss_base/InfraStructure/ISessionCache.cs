using rss_base.Models;

namespace rss_base.InfraStructure
{
    public interface ISessionCache
    {
        Session? AddOrUpdateSession(Session session);
        Session? GetSession(Guid id);
    }
}
