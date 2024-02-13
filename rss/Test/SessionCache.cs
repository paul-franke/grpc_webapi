using Xunit;

using rss_base.InfraStructure;
using rss_base.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Test;
public class SessionCacheTests 
{
    ISessionCache _sessions = new SessionCache(new MemoryCache(new MemoryCacheOptions()));


    [Fact]
    public void CheckInsertAndRetrievalFromCache()
    {
        //prepare
        var session = new Session
        {
            SessionId = System.Guid.NewGuid(),
            IpAddress = "127.0.0.1",
            AttendedAllowed = true,
            UnAttendedAllowed = true,
            sessionStatus = SessionStatus.New
        };

        //Act
        _sessions.AddOrUpdateSession(session);
        var session2 = _sessions.GetSession(session.SessionId); 

        //Check
        Assert.True(CompareSessions(session,session2));
    }

    [Fact]
    public void CheckTwoInsertsAndRetrievalFromCache()
    {
        //Prepare
        var session1 = new Session
        {
            SessionId = new System.Guid(),
            IpAddress = "127.0.0.1",
            AttendedAllowed = true,
            UnAttendedAllowed = true,
            sessionStatus = SessionStatus.New
        };
        var session2 = new Session
        {
            SessionId = new System.Guid(),
            IpAddress = "127.0.0.1",
            AttendedAllowed = true,
            UnAttendedAllowed = true,
            sessionStatus = SessionStatus.New
        };

        //Act
        _sessions.AddOrUpdateSession(session1);
        _sessions.AddOrUpdateSession(session2);

        var session11 = _sessions.GetSession(session1.SessionId);
        var session22 = _sessions.GetSession(session2.SessionId);
        Assert.True(CompareSessions(session1, session11) &&
                    CompareSessions(session2, session22));
    }

    private bool CompareSessions(Session session1, Session session2) 
    {
        return
            session1.SessionId.Equals(session2.SessionId) &&
            session1.IpAddress.Equals(session2.IpAddress) &&
            session1.AttendedAllowed.Equals(session2.AttendedAllowed) &&
            session1.UnAttendedAllowed.Equals(session2.UnAttendedAllowed) &&
            session1.sessionStatus.Equals(session2.sessionStatus);
    }
}
