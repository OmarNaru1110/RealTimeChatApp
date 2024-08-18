using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    [Owned]
    public class Connection
    {
        public string ConnectionId { get; set; }
        public int ReceiverId { get; set; }
    }
}
