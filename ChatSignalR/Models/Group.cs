namespace Server.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ICollection<User> Members { get; set; } = new List<User>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<GroupConnection> GroupConnections { get; set; }
    }
}
