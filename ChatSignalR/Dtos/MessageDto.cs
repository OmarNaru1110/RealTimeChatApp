using Server.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Dtos
{
    public class MessageDto
    {
        public string Content { get; set; }
        public int SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public int? GroupId { get; set; }
        public string? SenderUsername { get; set; }
    }
}
