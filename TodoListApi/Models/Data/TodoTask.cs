namespace TodoListApi.Models.Data
{
    public class TodoTask
    {
        public int TodoTaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }

    }
}