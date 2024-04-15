using System.Text.Json.Serialization;

namespace TodoListApi.Models.Data
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public string Role { get; set; }
        [JsonIgnore]
        public ICollection<TodoTask> Tasks {get;} = new List<TodoTask>();
    }
}