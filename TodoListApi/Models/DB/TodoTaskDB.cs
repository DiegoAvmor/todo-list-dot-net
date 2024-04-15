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
            Id=1,
            UserName="test1",
            Email="test1@gmail.com",
            Password="string",
            Role="user",
        }, new User{
            Id=2,
            UserName="test2",
            Email="test2@gmail.com",
            Password="string",
            Role="user",
        }, new User{
            Id=3,
            UserName="test3",
            Email="test3@gmail.com",
            Password="string",
            Role="admin",
        });
    }
        
    }
}