using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            .WithOpenApi();

            LoginEndpoints.MapGet("/tasks", getTasks)
            .WithName("GetTasks")
            .Produces<IResult>(200).Produces(400);

            LoginEndpoints.MapPost("/tasks", createTask)
            .WithName("CreateTask")
            .Produces<IResult>(200).Produces(400);

            LoginEndpoints.MapGet("/{id:int}/task", getTaskById)
            .WithName("GetTaskById")
            .Produces<IResult>(200).Produces(400);
        }

        static async Task<IResult> createTask(IMapper _map, [FromBody] TodoTaskRequestDTO requestDTO, ClaimsPrincipal claimsPrincipal, TodoTaskDB _db)
        {
            User user = await getUserFromToken(claimsPrincipal, _db);
            if (user == null){
                return Results.Conflict("It wasnt possible to retrieved the data from the user");
            }
            TodoTask newTask = _map.Map<TodoTask>(requestDTO);
            newTask.UserId = user.UserId;

            //Save new task
            _db.TodoTasks.Add(newTask);
            await _db.SaveChangesAsync();


            return Results.Ok(_map.Map<TodoTaskResponseDTO>(newTask));
        }

        //TODO: No task are being obtained as no user is being associated correctly to the task
        static async Task<IResult> getTasks(IMapper _map, ClaimsPrincipal claimsPrincipal, TodoTaskDB _db){
            User user = await getUserFromToken(claimsPrincipal, _db);
            //TODO: Improve mapping logic from user.tasks to list<TodoTaskDTO>
            //List<TodoTask> todoTasks = await _db.TodoTasks.FindAsync(user.UserId);
            List<TodoTaskDTO> taskDTOs = new List<TodoTaskDTO>();
            if(user.Tasks.Count > 0){
            foreach (var task in user.Tasks)
            {
                taskDTOs.Add(_map.Map<TodoTaskDTO>(task));
            }
            }
            return Results.Ok(taskDTOs);
        }

        static async Task<IResult> getTaskById(int id, IMapper _map, TodoTaskDB _db){
            TodoTask todoTask = await _db.TodoTasks.FirstOrDefaultAsync(x => x.TodoTaskId == id);
            if (todoTask ==null){
                return Results.NotFound($"Task {id} not found");
            }
            return Results.Ok(_map.Map<TodoTaskResponseDTO>(todoTask));
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