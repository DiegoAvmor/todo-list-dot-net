namespace TodoListApi.Models.Data.DTO
#nullable disable
{
    public class LoginRequestDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}