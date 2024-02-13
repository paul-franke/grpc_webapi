namespace rss_base.Models
{
    public class Session
    {
        public Guid SessionId { get; set; }
        public string? IpAddress {  get; set; }
        public string? UserId { get; set; }
        public SessionStatus sessionStatus { get; set; }
        public bool AttendedAllowed { get; set; }
        public bool UnAttendedAllowed { get; set; }
    }

    public enum SessionStatus 
    { 
        Opened=1,
        Closed=2,
        Approved=3,
        Rejected=4,
        TimedOut=5,
        Failed=6,
        New=7
    }
}
