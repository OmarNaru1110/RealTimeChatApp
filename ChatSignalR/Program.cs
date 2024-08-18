
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Server.Hubs;
using Server.Repositories;
using Server.Repositories.Context;
using Server.Repositories.IRepositories;

namespace ChatSignalR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IGroupRepository, GroupRepository>();
            builder.Services.AddSignalR();

            builder.Services.AddDbContext<ApplicationContext>(option =>
            {
                var connectionString = builder.Configuration.GetConnectionString("default");
                option.UseSqlServer(connectionString);
                option.EnableSensitiveDataLogging();
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            app.MapHub<ChatHub>("/ChatHub");
            app.MapHub<GroupChatHub>("/GroupChatHub");

            app.Run();
        }
    }
}
