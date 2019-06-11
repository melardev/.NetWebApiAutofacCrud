using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutofacCrud.Entities;
using WebApiAutofacCrud.Enums;

namespace WebApiAutofacCrud.Infrastructure.Services
{
    public interface ITodoService
    {
        Task<List<Todo>> FetchMany(TodoShow show = TodoShow.All);
        Task<Todo> Get(int todoId);
        Task<Todo> CreateTodo(Todo todo);
        Task<Todo> Update(int id, Todo todoFromUserInput);
        Task Delete(int todoId);
        Task DeleteAll();
        Task<Todo> Update(Todo todoFromDb, Todo todoFromUserInput);
        Task Delete(Todo todo);
    }
}