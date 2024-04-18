using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoListApi.Models.Data.DTO;

namespace TodoListApi.Tests;

public class AuthTests
{
    HttpClient client;

    [SetUp]
    public async Task Setup(){
        var application = new WebApplicationFactory<Program>();
        client = application.CreateClient();

        await client.PostAsJsonAsync("/api/auth/register", new RegistrationRequestDTO
        {
            UserName = "TestUser",
            Email = "sometest@gmail.com",
            Password = "testing1234"
        });
    }

    [TearDown]
    public void TearDown(){
        client?.Dispose();
    }

    [Test]
    public async Task RegisterValidUser()
    {
        var response = await client.PostAsJsonAsync("/api/auth/register", new RegistrationRequestDTO
        {
            UserName = "NewUser",
            Email = "newUser@gmail.com",
            Password = "testing1234"
        });
        Assert.NotNull(response);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task RegisterInvalidUser()
    {
        var response = await client.PostAsJsonAsync("/api/auth/register", new RegistrationRequestDTO
        {
            UserName = "",
            Email = "sometestgmail",
            Password = "testing1234"
        });
        Assert.NotNull(response);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity));
    }

    [Test]
    public async Task LoginValidUser()
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new RegistrationRequestDTO
        {
            UserName = "TestUser",
            Email = "sometest@gmail.com",
            Password = "testing1234"
        });
        Assert.NotNull(response);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task LoginInvalidUser()
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new RegistrationRequestDTO
        {
            UserName = "TestUser",
            Email = "sometest@gmail.com",
            Password = "wrongPassword"
        });
        Assert.NotNull(response);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }


}