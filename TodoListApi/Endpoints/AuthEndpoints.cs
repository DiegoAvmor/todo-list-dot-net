using TodoListApi.Models.DB;
using TodoListApi.Models.Data.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TodoListApi.Models.Data;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Serilog;
using O9d.AspNet.FluentValidation;
using TodoListApi.Config;
using TodoListApi.Utilities;

namespace TodoListApi.Endpoints
{
    public static class AuthEndpoints
    {
        public static void RegisterAuthEndpoints (this WebApplication app){

            var LoginEndpoints = app.MapGroup("/api/auth")
            .WithTags("Authentication")
            .WithValidationFilter()
            .WithOpenApi();

            LoginEndpoints.MapPost("/login", Login)
            .Accepts<LoginRequestDTO>("application/json")
            .Produces<IResult>(200).Produces(400).Produces(404);

            LoginEndpoints.MapPost("/register", Register)
            .Accepts<RegistrationRequestDTO>("application/json")
            .Produces<IResult>(200).Produces(400);

        }

        static async Task<IResult> Login(AesEncryption _encryptUtility, ApplicationConfig _configuration, IMapper _map,[Validate] [FromBody] LoginRequestDTO requestDTO, TodoTaskDB _db){
            User? user = await _db.Users.SingleOrDefaultAsync(x => x.UserName == requestDTO.UserName &&  x.Email == requestDTO.Email);
            if(user == null){
                return Results.NotFound("Failed login attempt: User not found error.");
            }

            if (_encryptUtility.Decrypt(user.Password) != requestDTO.Password){
                Log.Error("Failed login attempt: Invalid password.");
                return Results.BadRequest("Failed login attempt: Invalid username or password.");
            }
            //Generate JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.JwtConfig.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role)
                ]),
                Expires = DateTime.UtcNow.AddHours(_configuration.JwtConfig.ExpiresIn),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDTO loginResponseDTO  = new (){
                User = _map.Map<UserDTO>(user),
                Token = tokenHandler.WriteToken(token)
            };
            return Results.Ok(loginResponseDTO);
        }

        static async Task<IResult> Register(AesEncryption _encryptUtility, IMapper _map, [Validate] [FromBody] RegistrationRequestDTO requestDTO, TodoTaskDB _db){
            if(await _db.Users.FirstOrDefaultAsync(x => x.Email == requestDTO.Email && x.UserName ==requestDTO.UserName ) != null){
                return Results.BadRequest("Failed user registration: Usernamer or email already exists.");
            }

            User newUser = _map.Map<User>(requestDTO);
            newUser.Role = "user";
            newUser.Password = _encryptUtility.Encrypt(newUser.Password);

            _db.Users.Add(newUser);
            _db.SaveChanges();
            UserDTO newUserDTO = _map.Map<UserDTO>(newUser);
            return Results.Ok(newUserDTO);
        }
        
    }
}