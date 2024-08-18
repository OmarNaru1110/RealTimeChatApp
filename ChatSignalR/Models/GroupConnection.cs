using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    [Owned]
    public class GroupConnection
    {
        public string ConnectionId { get; set; }
        public int GroupId { get; set; }
    }
}
