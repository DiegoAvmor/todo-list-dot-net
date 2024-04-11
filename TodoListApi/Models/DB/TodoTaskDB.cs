using Microsoft.EntityFrameworkCore;
using TodoListApi.Models.Data;

namespace TodoListApi.Models.DB
{
    public class TodoTaskDB : DbContext
    {
        public TodoTaskDB(DbContextOptions<TodoTaskDB> options) : base(options) {

        }

    public DbSet<TodoTask> TodoTasks {get;set;}
        
    }
}