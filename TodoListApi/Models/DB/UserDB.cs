using Microsoft.EntityFrameworkCore;
using TodoListApi.Models.Data;

namespace TodoListApi.Models.DB
{
    public class UserDB : DbContext
    {
        public UserDB(DbContextOptions<UserDB> options) : base(options) {

        }
        
        public DbSet<User> Users {get;set;}
    }
}