

namespace rss_base.Controllers.Models
{
    public class SetSessionAllow
    {
        public Guid sessionId { get; set; }
        public bool Attended { get; set; }
        public bool UnAttended { get; set; }
    }
}
