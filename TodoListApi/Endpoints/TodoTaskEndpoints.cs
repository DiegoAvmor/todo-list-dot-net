using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using O9d.AspNet.FluentValidation;
using Serilog;
using TodoListApi.Models.Data;
using TodoListApi.Models.Data.DTO;
using TodoListApi.Models.DB;

namespace TodoListApi.Endpoints
{
    public static class TodoTaskEndpoints
    {
        public static void RegisterTodoTaskEndpoints (this WebApplication app){

            var LoginEndpoints = app.MapGroup("/api")
            .RequireAuthorization("User")
            .WithValidationFilter()
            .WithOpenApi();

            LoginEndpoints.MapGet("/tasks", getTasks)
            .WithName("GetTasks")
            .Produces<IResult>(200).Produces(409);

            LoginEndpoints.MapPost("/tasks", createTask)
            .WithName("CreateTask")
            .Produces<IResult>(200).Produces(409);

            LoginEndpoints.MapGet("/{id:int}/task", getTaskById)
            .WithName("GetTaskById")
            .Produces<IResult>(200).Produces(404);

            LoginEndpoints.MapDelete("/{id:int}/task", deleteTaskById)
            .WithName("DeleteTaskById")
            .Produces<IResult>(204).Produces(404);

            LoginEndpoints.MapPut("/{id:int}/task", updateTaskById)
            .WithName("UpdateTaskById")
            .Produces<IResult>(204).Produces(404);
        }

        static async Task<IResult> createTask(IMapper _map, [Validate] [FromBody] TodoTaskRequestDTO requestDTO, ClaimsPrincipal claimsPrincipal, TodoTaskDB _db)
        {
            Log.Information(requestDTO.ToString());
            User user = await getUserFromToken(claimsPrincipal, _db);
            if (user == null){
                Log.Error("User in token not found, invalid user.");
                return Results.Conflict("It wasnt possible to retrieved the data from the user");
            }

            try
            {
                TodoTask newTask = _map.Map<TodoTask>(requestDTO);
                newTask.UserId = user.Id;
                _db.TodoTasks.Add(newTask);
                await _db.SaveChangesAsync();
                return Results.Ok(_map.Map<TodoTaskResponseDTO>(newTask));
            }
            catch (Exception e)
            {
                Log.Error($"Failed to create task: {e.Message}");
                return Results.Problem(e.Message);
            }
        }

        static async Task<IResult> getTasks(IMapper _map, ClaimsPrincipal claimsPrincipal, TodoTaskDB _db){
            User user = await getUserFromToken(claimsPrincipal, _db);
            if (user == null){
                Log.Error("User in token not found, invalid user.");
                return Results.Conflict("It wasnt possible to retrieved the data from the user");
            }

            Log.Information($"Obtaining {user.UserName} tasks ....");
            List<TodoTask> todoTasks = _db.TodoTasks.Include(x => x.User)
                                        .Where(x => x.UserId == user.Id)
                                        .ToList();
            if (todoTasks.IsNullOrEmpty()){
                Log.Error("User todo tasks empty");
                return Results.Ok(todoTasks);
            }
            Log.Information($"Tasks obtained size: {todoTasks.Count}");
            List<TodoTaskResponseDTO> userTodoTasks = _map.Map<List<TodoTaskResponseDTO>>(todoTasks);      
            return Results.Ok(userTodoTasks);
        }

        static async Task<IResult> getTaskById(int id, ClaimsPrincipal claimsPrincipal, IMapper _map, TodoTaskDB _db){
            //Check if the task owner is the user from the token
            User user = await getUserFromToken(claimsPrincipal, _db);
            TodoTask? todoTask = await _db.TodoTasks.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);
            if (todoTask ==null){
                return Results.NotFound($"Task with id {id} not found");
            }
            return Results.Ok(_map.Map<TodoTaskResponseDTO>(todoTask));
        }

        static async Task<IResult> deleteTaskById(int id, ClaimsPrincipal claimsPrincipal, TodoTaskDB _db) {
            //Check if the task owner is the user from the token
            User user = await getUserFromToken(claimsPrincipal, _db);
            if (await _db.TodoTasks.FindAsync(id) is TodoTask todo)
            {
                if (todo.UserId == user.Id){
                    _db.TodoTasks.Remove(todo);
                    await _db.SaveChangesAsync();
                    return Results.NoContent();
                }
                Log.Error($"Failed to delete task: Request user is not owner of task {todo.Id}");
            }
            Log.Error($"Failed to delete task: Task with id {id} not found");
            return Results.NotFound($"Task with id {id} not found");
        }

        static async Task<IResult> updateTaskById(int id, [Validate] [FromBody] TodoTaskRequestDTO requestDTO, ClaimsPrincipal claimsPrincipal, TodoTaskDB _db) {
            //Check if the task owner is the user from the token
            User user = await getUserFromToken(claimsPrincipal, _db);
            if (await _db.TodoTasks.FindAsync(id) is TodoTask todo)
            {
                if (todo.UserId == user.Id){
                    todo.Title = requestDTO.Title;
                    todo.Description = requestDTO.Description;
                    await _db.SaveChangesAsync();
                    return Results.NoContent();
                }
                Log.Error($"Failed to update task: Request user is not owner of task {todo.Id}");
            }
            Log.Error($"Failed to update task: Task with id {id} not found");
            return Results.NotFound($"Task with id {id} not found");
        }

        private static async Task<User> getUserFromToken(ClaimsPrincipal claimsPrincipal, TodoTaskDB _db){
            try
            {
                string username = claimsPrincipal.Identity.Name;
                if (string.IsNullOrEmpty(username)){
                    throw new Exception($"No Username '{username}' associated in JWT");
                }
                Log.Information($"Current Request JWT User: {username}");
                User? user = await _db.Users.FirstAsync(x => x.UserName == username);
                if (user == null){
                    throw new Exception($"User '{username}' Not Found");
                }
                return user;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }
    }
}