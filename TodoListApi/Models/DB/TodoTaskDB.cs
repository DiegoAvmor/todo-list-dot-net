using Microsoft.EntityFrameworkCore;
using TodoListApi.Models.Data;

namespace TodoListApi.Models.DB
{
    public class TodoTaskDB : DbContext
    {
        public TodoTaskDB(DbContextOptions<TodoTaskDB> options) : base(options) {

        }

    public DbSet<TodoTask> TodoTasks {get;set;}
    public DbSet<User> Users {get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoTask>()
        .HasOne(e => e.User)
        .WithMany(e => e.Tasks)
        .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<User>().HasData(new User(){
            UserId=1,
            UserName="Diego",
            Email="diego@gmail.com",
            Password="string",
            Role="User",
        });
    }
        
    }
}