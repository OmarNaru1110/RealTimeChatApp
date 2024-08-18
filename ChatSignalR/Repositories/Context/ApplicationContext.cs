using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Repositories.Context
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(u =>
            {
                u.HasMany(m => m.SentMessages)
                .WithOne(m => m.Sender);
                
                u.HasMany(m => m.ReceivedMessages)
                .WithOne(m => m.Receiver);

                u.HasMany(g => g.Groups)
                .WithMany(g => g.Members);

                u.OwnsMany(c => c.Connections);

            });

            builder.Entity<Group>(g =>
            {
                g.HasMany(m => m.Messages)
                .WithOne(g => g.Group);

                g.OwnsMany(c => c.GroupConnections);
            });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
    }
}
