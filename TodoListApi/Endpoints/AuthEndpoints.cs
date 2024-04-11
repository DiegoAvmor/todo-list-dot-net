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
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace TodoListApi.Endpoints
{
    public static class AuthEndpoints
    {
        public static void RegisterAuthEndpoints (this WebApplication app){

            var LoginEndpoints = app.MapGroup("/api/auth")
            .WithOpenApi();

            LoginEndpoints.MapPost("/login", Login)
            .WithName("Login")
            .Accepts<LoginRequestDTO>("application/json")
            .Produces<IResult>(200).Produces(400);

            LoginEndpoints.MapPost("/register", Register)
            .WithName("Register")
            .Accepts<RegistrationRequestDTO>("application/json")
            .Produces<IResult>(200).Produces(400);

        }

        static async Task<IResult> Login(IConfiguration _configuration, IMapper _map,LoginRequestDTO requestDTO, UserDB _db){
            User? user = await _db.Users.SingleOrDefaultAsync(x => x.UserName == requestDTO.UserName && x.Password == requestDTO.Password);
            if(user == null){
                return Results.BadRequest("Wrong username or password");
            }
            //Generate JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role)
                ]),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDTO loginResponseDTO  = new (){
                User = _map.Map<UserDTO>(user),
                Token = tokenHandler.WriteToken(token)
            };
            return Results.Ok(loginResponseDTO);
        }

        static async Task<IResult> Register(IMapper _map, [FromBody] RegistrationRequestDTO requestDTO, UserDB _db){
            if(await _db.Users.FirstOrDefaultAsync(x => x.Email == requestDTO.Email && x.UserName ==requestDTO.UserName ) != null){
                return Results.BadRequest("User already exists");
            }

            User newUser = _map.Map<User>(requestDTO);
            newUser.Role = "user";
            _db.Users.Add(newUser);
            _db.SaveChanges();
            UserDTO newUserDTO = _map.Map<UserDTO>(newUser);
            return Results.Ok(newUserDTO);
        }
        
    }
}