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
        UserDto userDto;

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
            
            //Authenticate user
            var userResponse = await client.PostAsJsonAsync("/api/auth/login", user);
            var userResult = await userResponse.Content.ReadAsStringAsync();
            var responseUserToken = JsonConvert.DeserializeObject<LoginResponseDto>(userResult);
            userDto = responseUserToken!.User;
            userToken = $"{responseUserToken.Type} {responseUserToken.Token}";
        }

        [TearDown]
        public void TearDown(){
            client?.Dispose();
        }


        [Test]
        public async Task EditUserByOwner (){
            client.DefaultRequestHeaders.Add("Authorization", userToken);

            RegistrationRequestDto editedUserInformation = new RegistrationRequestDto
            {
                UserName = "HeyThisChanged!",
                Email = "thischanged@gmail.com",
                Password = "passwordChange!"
            };

            var response = await client.PutAsJsonAsync($"/api/users/{userDto.Id}", editedUserInformation);
            Assert.NotNull(response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public async Task EditInvalidUserByOwner (){
            client.DefaultRequestHeaders.Add("Authorization", userToken);

            RegistrationRequestDto editedUserInformation = new RegistrationRequestDto
            {
                UserName = "HeyThisChanged!",
                Email = "@gmail.com",
                Password = "passwordChangeAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA!"
            };

            var response = await client.PutAsJsonAsync($"/api/users/{userDto.Id}", editedUserInformation);
            Assert.NotNull(response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity));
            
        }

        [Test]
        public async Task EditNonExistentUserByOwner (){
            client.DefaultRequestHeaders.Add("Authorization", userToken);

            RegistrationRequestDto editedUserInformation = new RegistrationRequestDto
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

            var response = await client.DeleteAsync($"/api/users/{userDto.Id}");
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