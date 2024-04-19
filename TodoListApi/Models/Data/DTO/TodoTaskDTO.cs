namespace TodoListApi.Models.Data.DTO
#nullable disable
{
    public class TodoTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public User User { get; set; }

        
    }
}