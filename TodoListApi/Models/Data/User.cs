using System.Text.Json.Serialization;
#nullable disable
namespace TodoListApi.Models.Data
{
    public class User
    {
        public int Id { get; set; }
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