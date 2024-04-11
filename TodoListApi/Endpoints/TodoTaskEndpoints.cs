using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListApi.Endpoints
{
    public static class TodoTaskEndpoints
    {
        public static void RegisterTodoTaskEndpoints (this WebApplication app){

            var LoginEndpoints = app.MapGroup("/api/task")
            .RequireAuthorization("User")
            .WithOpenApi();

            LoginEndpoints.MapPost("/hello", Hello)
            .WithName("Hello")
            .Produces<IResult>(200).Produces(400);
        }

        static async Task<IResult> Hello(){

            return Results.Ok("Helllooooooo");
        }
    }
}