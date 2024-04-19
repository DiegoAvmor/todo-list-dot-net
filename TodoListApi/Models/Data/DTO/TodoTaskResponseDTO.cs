namespace TodoListApi.Models.Data.DTO
#nullable disable
{
    public class TodoTaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public UserDto user { get; set; }
    }
}