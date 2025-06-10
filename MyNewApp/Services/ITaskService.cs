using MyNewApp.Models;
using System.Collections.Generic;

namespace MyNewApp.Services;

/// <summary>
/// Interface: CURD for Todo
/// </summary>
public interface ITaskService
{
    Todo? GetTodoById(int id);
    List<Todo> GetTodos();
    void DeleteTodoById(int id);
    Todo AddTodo(Todo task);
}
