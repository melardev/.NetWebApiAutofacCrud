using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using WebApiAutofacCrud.Entities;
using Database = System.Data.Entity.Database;

namespace WebApiAutofacCrud.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Todo> Todos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public override int SaveChanges()
        {
            HandleTimeStamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            HandleTimeStamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void HandleTimeStamps()
        {
            var now = DateTime.Now;
            foreach (var dbEntityEntry in ChangeTracker.Entries())
                if (dbEntityEntry.Entity is ITimestampedEntity entity)
                    switch (dbEntityEntry.State)
                    {
                        case EntityState.Added:
                            if (entity.CreatedAt == null)
                                entity.CreatedAt = now;
                            if (entity.UpdatedAt == null) entity.UpdatedAt = now;

                            break;
                        case EntityState.Modified:
                            Entry(entity).Property(e => e.CreatedAt).IsModified = false;
                            entity.UpdatedAt = now;
                            break;
                    }
        }


        public static async void Seed()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var todosCount = context.Todos.Count();
                var todosToSeed = 32;
                todosToSeed -= todosCount;
                if (todosToSeed > 0)
                {
                    Console.WriteLine($"[+] Seeding ${todosToSeed} Todos");
                    var faker = new Faker<Todo>()
                        .RuleFor(a => a.Title, f => string.Join(" ", f.Lorem.Words(f.Random.Int(2, 5))))
                        .RuleFor(a => a.Description, f => f.Lorem.Sentences(f.Random.Int(1, 10)))
                        .RuleFor(t => t.Completed, f => f.Random.Bool(0.4f))
                        .RuleFor(a => a.CreatedAt,
                            f => f.Date.Between(DateTime.Now.AddYears(-5), DateTime.Now.AddDays(-1)))
                        .FinishWith((f, todoInstance) =>
                        {
                            todoInstance.UpdatedAt =
                                f.Date.Between(todoInstance.CreatedAt.Value, DateTime.Now);
                        });

                    var todos = faker.Generate(todosToSeed);
                    context.Todos.AddRange(todos);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}