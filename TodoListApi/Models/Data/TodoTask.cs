namespace TodoListApi.Models.Data
{
    public class TodoTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public User user { get; set; }

    }
}