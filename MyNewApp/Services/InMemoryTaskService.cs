using MyNewApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyNewApp.Services;

/// <summary>
/// To save all Todo objects into List
/// </summary>
public class InMemoryTaskService : ITaskService
{
    private readonly List<Todo> _todos = new();

    public Todo AddTodo(Todo task)
    {
        _todos.Add(task);
        return task;
    }

    public void DeleteTodoById(int id)
    {
        _todos.RemoveAll(t => t.Id == id);
    }

    public Todo? GetTodoById(int id)
    {
        return _todos.SingleOrDefault(t => t.Id == id);
    }

    public List<Todo> GetTodos() => _todos;
}
