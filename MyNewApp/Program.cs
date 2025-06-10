using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;
using MyNewApp.Models;
using MyNewApp.Services;

var builder = WebApplication.CreateBuilder(args);

// In DI container register ITaskService
builder.Services.AddSingleton<ITaskService, InMemoryTaskService>();

var app = builder.Build();

// rewrite the route tasks -> todos
app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));

// add our own custom middleware.
app.Use(async (context, next) =>
{
    Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Started.");
    await next();
    Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Finished.");
});

// inject ITaskService£¬do CRUD
app.MapGet("/todos", (ITaskService svc) => svc.GetTodos());

app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound>(int id, ITaskService svc) =>
{
    var todo = svc.GetTodoById(id);
    return todo is null ? TypedResults.NotFound() : TypedResults.Ok(todo);
});

app.MapPost("/todos", (Todo task, ITaskService svc) =>
{
    var created = svc.AddTodo(task);
    return TypedResults.Created($"/todos/{created.Id}", created);
})
.AddEndpointFilter(async (context, next) =>
{
    var taskArg = context.GetArgument<Todo>(0)!;
    var errors = new Dictionary<string, string[]>();

    if (taskArg.DueDate < DateTime.UtcNow)
        errors[nameof(Todo.DueDate)] = new[] { "Cannot have due date in the past." };
    if (taskArg.IsCompleted)
        errors[nameof(Todo.IsCompleted)] = new[] { "Cannot add completed todo." };

    if (errors.Count > 0)
        return Results.ValidationProblem(errors);

    return await next(context);
});

app.MapDelete("/todos/{id}", (int id, ITaskService svc) =>
{
    svc.DeleteTodoById(id);
    return TypedResults.NoContent();
});

app.Run();
