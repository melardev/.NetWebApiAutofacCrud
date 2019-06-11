using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutofacCrud.Data;
using WebApiAutofacCrud.Entities;
using WebApiAutofacCrud.Enums;

namespace WebApiAutofacCrud.Infrastructure.Services
{
    public class TodoService : ITodoService
    {
        private readonly ApplicationDbContext _dbContext;

        public TodoService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Todo>> FetchMany(TodoShow show = TodoShow.All)
        {
            // Retrieve hwo many articles with our criteria(All, Completed or Pending)
            IQueryable<Todo> queryable = _dbContext.Todos.OrderBy(t => t.CreatedAt);

            if (show == TodoShow.Completed)
                queryable = queryable.Where(t => t.Completed);
            else if (show == TodoShow.Pending) queryable = queryable.Where(t => !t.Completed);


            List<Todo> todos;
            if (show != TodoShow.All)
                // https://stackoverflow.com/questions/5325797/the-entity-cannot-be-constructed-in-a-linq-to-entities-query
                // for complete/pending
                todos = (await queryable
                        .Select(t => new // let's create an anonymous type
                        {
                            // t.Id is the same as Id = t.Id
                            t.Id,
                            t.Title,
                            t.Completed,
                            t.CreatedAt,
                            t.UpdatedAt
                        })
                        .ToListAsync())
                    .Select(anonymousType => new Todo
                    {
                        Id = anonymousType.Id,
                        Title = anonymousType.Title,
                        Completed = anonymousType.Completed,
                        CreatedAt = anonymousType.CreatedAt,
                        UpdatedAt = anonymousType.UpdatedAt
                    })
                    .ToList();
            else
                todos = await queryable.ToListAsync();

            return todos;
        }


        public async Task<Todo> Get(int todoId)
        {
            return await _dbContext.Todos.FirstOrDefaultAsync(t => t.Id == todoId);
        }

        public async Task<Todo> CreateTodo(Todo todo)
        {
            _dbContext.Todos.Add(todo);
            await _dbContext.SaveChangesAsync();
            return todo;
        }

        public async Task<Todo> Update(int id, Todo todoFromUserInput)
        {
            var todoFromDb = _dbContext.Todos.First(t => t.Id == id);
            todoFromDb.Title = todoFromUserInput.Title;
            if (todoFromUserInput.Description != null)
                todoFromDb.Description = todoFromUserInput.Description;
            todoFromDb.Completed = todoFromUserInput.Completed;

            // Not needed, it is set in ApplicationDbContext
            _dbContext.Entry(todoFromDb).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return todoFromDb;
        }


        public async Task Delete(int todoId)
        {
            var todoFromDb = await _dbContext.Todos.FirstAsync(t => t.Id == todoId);
            _dbContext.Todos.Remove(todoFromDb);
            _dbContext.Entry(todoFromDb).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAll()
        {
            _dbContext.Todos.RemoveRange(_dbContext.Todos);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Todo> Update(Todo todoFromDb, Todo todoFromUserInput)
        {
            todoFromDb.Title = todoFromUserInput.Title;
            if (todoFromUserInput.Description != null)
                todoFromDb.Description = todoFromUserInput.Description;
            todoFromDb.Completed = todoFromUserInput.Completed;

            // Not needed, it is set in ApplicationDbContext
            _dbContext.Entry(todoFromDb).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return todoFromDb;
        }

        public async Task Delete(Todo todo)
        {
            _dbContext.Todos.Remove(todo);
            await _dbContext.SaveChangesAsync();
        }
    }
}