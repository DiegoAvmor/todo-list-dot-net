namespace TodoListApi.Models.Data.DTO
{
    public class TodoTaskResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public UserDTO user { get; set; }
    }
}