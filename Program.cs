using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace DisposableProject
{
    [UsedImplicitly]
    class Program
    {
        static void Main()
        {
            Run().GetAwaiter().GetResult();

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static async Task Run()
        {
            using (var context = new SpecialisedContext())
            {
                var repo = new ItemRepository(context);
                
                foreach (var x in await repo.GetAll())
                    Console.WriteLine(x.Id + ": " + x.Name);

                foreach (var x in await repo.GetSomethingElse())
                    Console.WriteLine(x);
            }
        }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ItemRepository : EntityFrameworkRepository<Item, int>, IItemRepository
    {
        private readonly SpecialisedContext _context;

        public ItemRepository(SpecialisedContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<Item>> GetSomethingElse()
        {
            return Task.FromResult(new List<Item>() { new Item() { Id = 1, Name = "Some" } });
        }

        public override string ToString()
        {
            return _context == null ? "oops" : "yey";
        }
    }

    public interface IItemRepository : IEntityRepository<Item, int>
    {
        Task<List<Item>> GetSomethingElse();
    }

    public interface IEntityRepository<TEntity, in TKey> where TEntity : class
    {
        Task<List<TEntity>> GetAll();
        Task<TEntity> Get(TKey id);
    }

    public class EntityFrameworkRepository<TEntity, TKey> where TEntity : class
    {
        private readonly DbContext _context;

        protected EntityFrameworkRepository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> Get(TKey id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }
    }

    public class SpecialisedContext : DbContext
    {
        public SpecialisedContext() : base("conString")
        {

        }
    }
}
