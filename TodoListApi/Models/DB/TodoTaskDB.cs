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
        modelBuilder.Entity<User>()
        .HasMany(e => e.Tasks)             //TodoTask Has one User
        .WithOne(e => e.User)              //That User has many Tasks
        .HasForeignKey(e => e.UserId)      //UserId is the Foreign Key of User
        .OnDelete(DeleteBehavior.Cascade);

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