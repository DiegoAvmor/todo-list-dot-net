namespace TodoListApi.Models.Data.DTO
{
    public class TodoTaskDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public User user { get; set; }

        
    }
}