using Microsoft.EntityFrameworkCore;
using TodoListApi.Models.Data;
using TodoListApi.Utilities;

namespace TodoListApi.Models.DB
{
    public class TodoTaskDB : DbContext
    {
        private readonly AesEncryption encryptionUtility;
        public TodoTaskDB(DbContextOptions<TodoTaskDB> options, AesEncryption _aesEncryption) : base(options) {
            encryptionUtility = _aesEncryption;
        }

    public DbSet<TodoTask> TodoTasks {get;set;}
    public DbSet<User> Users {get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
        .HasMany(e => e.Tasks)             //User has many TodoTasks
        .WithOne(e => e.User)              //TodoTask is associated with only one User
        .HasForeignKey(e => e.UserId)      //The foreign key
        .OnDelete(DeleteBehavior.Cascade);

        //Create Dummy Users in DB
        modelBuilder.Entity<User>().HasData(new User(){
            Id=1,
            UserName="test1",
            Email="test1@gmail.com",
            Password=encryptionUtility.Encrypt("root"),
            Role="user",
        }, new User{
            Id=2,
            UserName="test2",
            Email="test2@gmail.com",
            Password=encryptionUtility.Encrypt("root"),
            Role="user",
        }, new User{
            Id=3,
            UserName="test3",
            Email="test3@gmail.com",
            Password=encryptionUtility.Encrypt("root"),
            Role="admin",
        });

        //Create Dummy Tasks
        modelBuilder.Entity<TodoTask>().HasData(new TodoTask(){
            Id=1,
            Title="This is my task!",
            Description="This is some dummy text",
            UserId=1,
        }, new TodoTask(){
            Id=2,
            Title="This is my task!",
            Description="This is some dummy text",
            UserId=2,
        }, new TodoTask(){
            Id=3,
            Title="This is my task!",
            Description="This is some dummy text",
            UserId=3,
        });
    }
        
    }
}