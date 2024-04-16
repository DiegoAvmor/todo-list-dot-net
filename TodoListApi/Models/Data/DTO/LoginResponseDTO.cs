namespace TodoListApi.Models.Data.DTO
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
        public string Type { get; set; } = "Bearer";
        
    }
}