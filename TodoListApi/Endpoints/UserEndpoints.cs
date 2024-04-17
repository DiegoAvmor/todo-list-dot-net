using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using O9d.AspNet.FluentValidation;
using Serilog;
using TodoListApi.Models.Data;
using TodoListApi.Models.Data.DTO;
using TodoListApi.Models.DB;
using TodoListApi.Utilities;

namespace TodoListApi.Endpoints
{
    public static class UserEndpoints
    {
        public static void RegisterUserManagementEndpoints(this WebApplication app){
            var userEndpoints = app.MapGroup("/api/user")
            .RequireAuthorization("User")
            .WithValidationFilter()
            .WithOpenApi();

            userEndpoints.MapPut("/{id:int}", EditUser)
            .WithName("EditUser")
            .Accepts<RegistrationRequestDTO>("application/json")
            .Produces<IResult>(200).Produces(404).Produces(409);

            userEndpoints.MapDelete("/{id:int}", DeleteUser)
            .WithName("DeleteUser")
            .Produces<IResult>(200).Produces(404).Produces(409);
        }

        private static async Task<IResult> DeleteUser(int id, ClaimsPrincipal claimsPrincipal, TodoTaskDB _db){
            //Check if the task owner is the user from the token
            User user = await TokenUtility.GetUserFromToken(claimsPrincipal, _db);
             if (user == null){
                return Results.Conflict("It wasnt possible to retrieved the data from the user");
            }
            if (await _db.Users.FindAsync(id) is User _user)
            {
                if(user.Id == _user.Id){
                    _db.Users.Remove(_user);
                    await _db.SaveChangesAsync();
                    return Results.NoContent();
                }
                Log.Error($"Failed to delete user: Request user is not owner of user id {_user.Id}");
            }
            Log.Error($"Failed to delete user: User with id {id} not found");
            return Results.NotFound("Failed to delete user: User not found or doesnt exist");
            throw new NotImplementedException();
        }

        public static async Task<IResult> EditUser(int id, [Validate] [FromBody] RegistrationRequestDTO requestDTO, AesEncryption _encryptUtility, ClaimsPrincipal claimsPrincipal, TodoTaskDB _db){   
            //Check if the task owner is the user from the token
            User user = await TokenUtility.GetUserFromToken(claimsPrincipal, _db);
            if (user == null){
                return Results.Conflict("It wasnt possible to retrieved the data from the user");
            }

            if (await _db.Users.FindAsync(id) is User _user)
            {
                if (user.Id == _user.Id){
                    _user.UserName = requestDTO.UserName;
                    _user.Email = requestDTO.Email;
                    _user.Password = _encryptUtility.Encrypt(requestDTO.Password);
                    await _db.SaveChangesAsync();
                    return Results.NoContent();
                }
                Log.Error($"Failed to update user: Request user is not owner of user id {_user.Id}");
            }
            Log.Error($"Failed to update user: User with id {id} not found");
            return Results.NotFound($"Failed to update user: User not found or doesnt exist");
        }

    }
}