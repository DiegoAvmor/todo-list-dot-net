using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TodoListApi.Models.Data.DTO;

namespace TodoListApi.Tests
{
    public class TodoTasksTests
    {
        HttpClient client;
        string token;
        TodoTaskRequestDTO task = new TodoTaskRequestDTO
        {
            Title = "This is a dummy task",
            Description = "This is a dummy description"
            
        };

        [SetUp]
        public async Task Setup(){
            var application = new WebApplicationFactory<Program>();
            client = application.CreateClient();
            var user = new RegistrationRequestDTO
            {
                UserName = "TestUser",
                Email = "sometest@gmail.com",
                Password = "testing1234"
            };

            //Generate dummy user
            await client.PostAsJsonAsync("/api/auth/register", user);
            //Authenticate dummy user
            var response = await client.PostAsJsonAsync("/api/auth/login", user);
            var result = await response.Content.ReadAsStringAsync();

            var responseToken = JsonConvert.DeserializeObject<LoginResponseDTO>(result);
            token = $"{responseToken.Type} {responseToken.Token}";
        }

        [TearDown]
        public void TearDown(){
            client?.Dispose();
        }

        [Test]
        public async Task getUserTodoTasks()
        {
            client.DefaultRequestHeaders.Add("Authorization", token);
            await client.PostAsJsonAsync("/api/tasks", task);

            var response = await client.GetAsync("/api/tasks");
            Assert.NotNull(response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var result = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<List<TodoTaskResponseDTO>>(result);
            Assert.That(deserializedResponse.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task getUserTodoTasksById()
        {
            client.DefaultRequestHeaders.Add("Authorization", token);
            var createdTask = await client.PostAsJsonAsync("/api/tasks", task);
            var createdTaskResult = await createdTask.Content.ReadAsStringAsync();
            var todoTaskResponseDTO = JsonConvert.DeserializeObject<TodoTaskResponseDTO>(createdTaskResult);

            var response = await client.GetAsync($"/api/{todoTaskResponseDTO.Id}/task");
            Assert.NotNull(response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var result = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<TodoTaskResponseDTO>(result);
            Assert.That(deserializedResponse.Id, Is.EqualTo(todoTaskResponseDTO.Id));
        }

        [Test]
        public async Task updateTodoTasksById()
        {
            client.DefaultRequestHeaders.Add("Authorization", token);
            var createdTask = await client.PostAsJsonAsync("/api/tasks", task);
            var createdTaskResult = await createdTask.Content.ReadAsStringAsync();
            var todoTaskResponseDTO = JsonConvert.DeserializeObject<TodoTaskResponseDTO>(createdTaskResult);

            task.Title = "This title changed!";
            task.Description = "Hi this is a changed description!";

            var response = await client.PutAsJsonAsync($"/api/{todoTaskResponseDTO.Id}/task", task);
            Assert.NotNull(response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public async Task deleteTodoTasksById()
        {
            client.DefaultRequestHeaders.Add("Authorization", token);
            var createdTask = await client.PostAsJsonAsync("/api/tasks", task);
            var createdTaskResult = await createdTask.Content.ReadAsStringAsync();
            var todoTaskResponseDTO = JsonConvert.DeserializeObject<TodoTaskResponseDTO>(createdTaskResult);

            var response = await client.DeleteAsync($"/api/{todoTaskResponseDTO.Id}/task");
            Assert.NotNull(response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}