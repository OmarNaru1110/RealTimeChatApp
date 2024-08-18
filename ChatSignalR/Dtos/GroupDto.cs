using Server.Models;

namespace Server.Dtos
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ICollection<UsernameAndIdDto> Members { get; set; } = new List<UsernameAndIdDto>();
    }
}
