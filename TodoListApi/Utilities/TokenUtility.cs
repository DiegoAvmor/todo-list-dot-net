using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TodoListApi.Models.Data;
using TodoListApi.Models.DB;

namespace TodoListApi.Utilities
{
    public static class TokenUtility
    {
        public static async Task<User?> GetUserFromToken(ClaimsPrincipal claimsPrincipal, TodoTaskDB _db){
            try
            {
                string? username = claimsPrincipal.Identity?.Name;
                if (string.IsNullOrEmpty(username)){
                    throw new Exception($"No Username '{username}' associated in JWT");
                }

                User? user = await _db.Users.FirstAsync(x => x.UserName == username) ?? throw new Exception($"User '{username}' Not Found");
                return user;
            }
            catch (Exception e)
            {
                Log.Error($"Failed user token extration: {e.Message}");
                return null;
            }
        }
    }
}