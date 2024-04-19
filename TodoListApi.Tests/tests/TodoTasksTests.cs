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
        readonly TodoTaskRequestDto task = new TodoTaskRequestDto
        {
            Title = "This is a dummy task",
            Description = "This is a dummy description"
            
        };

        [SetUp]
        public async Task Setup(){
            var application = new WebApplicationFactory<Program>();
            client = application.CreateClient();
            var user = new RegistrationRequestDto
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

            var responseToken = JsonConvert.DeserializeObject<LoginResponseDto>(result);
            token = $"{responseToken!.Type} {responseToken.Token}";
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
            var deserializedResponse = JsonConvert.DeserializeObject<List<TodoTaskResponseDto>>(result);
            Assert.That(deserializedResponse!.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task getUserTodoTasksById()
        {
            client.DefaultRequestHeaders.Add("Authorization", token);
            var createdTask = await client.PostAsJsonAsync("/api/tasks", task);
            var createdTaskResult = await createdTask.Content.ReadAsStringAsync();
            var todoTaskResponseDto = JsonConvert.DeserializeObject<TodoTaskResponseDto>(createdTaskResult);

            var response = await client.GetAsync($"/api/{todoTaskResponseDto!.Id}/task");
            Assert.NotNull(response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var result = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<TodoTaskResponseDto>(result);
            Assert.That(deserializedResponse!.Id, Is.EqualTo(todoTaskResponseDto.Id));
        }

        [Test]
        public async Task updateTodoTasksById()
        {
            client.DefaultRequestHeaders.Add("Authorization", token);
            var createdTask = await client.PostAsJsonAsync("/api/tasks", task);
            var createdTaskResult = await createdTask.Content.ReadAsStringAsync();
            var todoTaskResponseDto = JsonConvert.DeserializeObject<TodoTaskResponseDto>(createdTaskResult);

            task.Title = "This title changed!";
            task.Description = "Hi this is a changed description!";

            var response = await client.PutAsJsonAsync($"/api/{todoTaskResponseDto!.Id}/task", task);
            Assert.NotNull(response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public async Task deleteTodoTasksById()
        {
            client.DefaultRequestHeaders.Add("Authorization", token);
            var createdTask = await client.PostAsJsonAsync("/api/tasks", task);
            var createdTaskResult = await createdTask.Content.ReadAsStringAsync();
            var todoTaskResponseDto = JsonConvert.DeserializeObject<TodoTaskResponseDto>(createdTaskResult);

            var response = await client.DeleteAsync($"/api/{todoTaskResponseDto!.Id}/task");
            Assert.NotNull(response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}