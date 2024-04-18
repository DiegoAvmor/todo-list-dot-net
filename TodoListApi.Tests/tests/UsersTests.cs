using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TodoListApi.Models.Data.DTO;

namespace TodoListApi.Tests
{
    public class UsersTests
    {
        HttpClient client;
        string userToken;
        UserDTO userDTO;

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
            
            //Authenticate user
            var userResponse = await client.PostAsJsonAsync("/api/auth/login", user);
            var userResult = await userResponse.Content.ReadAsStringAsync();
            var responseUserToken = JsonConvert.DeserializeObject<LoginResponseDTO>(userResult);
            userDTO = responseUserToken.User;
            userToken = $"{responseUserToken.Type} {responseUserToken.Token}";

            //Authenticate admin
            //var adminResponse = await client.PostAsJsonAsync("/api/auth/login", admin);
            //var adminResult = await adminResponse.Content.ReadAsStringAsync();
            //var responseAdminToken = JsonConvert.DeserializeObject<LoginResponseDTO>(adminResult);
            //adminToken = $"{responseAdminToken.Type} {responseAdminToken.Token}";
        }

        [TearDown]
        public void TearDown(){
            client?.Dispose();
        }


        [Test]
        public async Task EditUserByOwner (){
            client.DefaultRequestHeaders.Add("Authorization", userToken);

            RegistrationRequestDTO editedUserInformation = new RegistrationRequestDTO
            {
                UserName = "HeyThisChanged!",
                Email = "thischanged@gmail.com",
                Password = "passwordChange!"
            };

            var response = await client.PutAsJsonAsync($"/api/users/{userDTO.Id}", editedUserInformation);
            Assert.NotNull(response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public async Task EditInvalidUserByOwner (){
            client.DefaultRequestHeaders.Add("Authorization", userToken);

            RegistrationRequestDTO editedUserInformation = new RegistrationRequestDTO
            {
                UserName = "HeyThisChanged!",
                Email = "@gmail.com",
                Password = "passwordChangeAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA!"
            };

            var response = await client.PutAsJsonAsync($"/api/users/{userDTO.Id}", editedUserInformation);
            Assert.NotNull(response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity));
            
        }

        [Test]
        public async Task EditNonExistentUserByOwner (){
            client.DefaultRequestHeaders.Add("Authorization", userToken);

            RegistrationRequestDTO editedUserInformation = new RegistrationRequestDTO
            {
                UserName = "HeyThisChanged!",
                Email = "thischanged@gmail.com",
                Password = "passwordChange!"
            };

            var response = await client.PutAsJsonAsync($"/api/users/{0}", editedUserInformation);
            Assert.NotNull(response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            
        }

        [Test]
        public async Task DeleteExistentUserByOwner (){
            client.DefaultRequestHeaders.Add("Authorization", userToken);

            var response = await client.DeleteAsync($"/api/users/{userDTO.Id}");
            Assert.NotNull(response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public async Task DeleteNonExistentUserByOwner (){
            client.DefaultRequestHeaders.Add("Authorization", userToken);

            var response = await client.DeleteAsync($"/api/users/{0}");
            Assert.NotNull(response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}