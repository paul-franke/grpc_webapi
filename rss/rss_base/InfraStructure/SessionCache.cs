using Microsoft.Extensions.Caching.Memory;
using rss_base.Models;

namespace rss_base.InfraStructure
{
    public class SessionCache : ISessionCache
    {
        private readonly IMemoryCache _memoryCache;

        public SessionCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Session? AddOrUpdateSession(Session session)
        {
             var cacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
            _memoryCache.Set(session.SessionId, session, cacheEntryOptions);
            return GetSession(session.SessionId);
        }

        public Session? GetSession(Guid id)
        {
            if (!_memoryCache.TryGetValue(id, out Session cacheValue))
            {
                return null;
            }
            return cacheValue;
        }
    }
}
