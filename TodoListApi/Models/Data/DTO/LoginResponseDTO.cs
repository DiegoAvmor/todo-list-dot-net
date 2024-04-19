namespace TodoListApi.Models.Data.DTO
#nullable disable
{
    public class LoginResponseDto
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
        public string Type { get; set; } = "Bearer";
        
    }
}