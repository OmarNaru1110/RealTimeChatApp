using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [ForeignKey(nameof(Sender))]
        public int SenderId { get; set; }
        public User Sender { get; set; }
        [ForeignKey(nameof(Receiver))]
        public int? ReceiverId { get; set; }
        public User? Receiver { get; set; }
        [ForeignKey(nameof(Group))]
        public int? GroupId { get; set; }
        public Group? Group { get; set; }
    }
}
