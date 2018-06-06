using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;

namespace DisposableProject
{
    [UsedImplicitly]
    class Program
    {
        static void Main()
        {
            using (var repo = new ItemRepository(new SpecialisedContext()))
            {
                foreach (var x in repo.GetAll())
                    Console.WriteLine(x.Id + ": " + x.Name);

                foreach(var x in repo.GetSomethingElse())
                    Console.WriteLine(x);
            }

            Console.WriteLine("Done");
            Console.ReadLine();
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
        public ItemRepository(DbContext context) : base(context)
        {
        }

        public List<Item> GetSomethingElse()
        {
            return new List<Item>() {new Item() {Id = 1, Name = "Some"}};
        }
    }

    public interface IItemRepository : IEntityRepository<Item, int>
    {
        List<Item> GetSomethingElse();
    }

    public interface IEntityRepository<TEntity, in TKey> where TEntity : class
    {
        List<TEntity> GetAll();
        TEntity Get(TKey id);
    }

    public class EntityFrameworkRepository<TEntity, TKey> : IDisposable where TEntity : class
    {
        private readonly DbContext _context;

        protected EntityFrameworkRepository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public List<TEntity> GetAll()
        {
            return _context.GetAll<TEntity>();
        }
        
        public TEntity Get(TKey id)
        {
            Console.WriteLine(id);
            return _context.Get<TEntity>();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }

    public class SpecialisedContext : DbContext
    {
        public SpecialisedContext() : base("conString")
        {

        }
    }

    public abstract class DbContext : IDisposable
    {
        private readonly string _conStr;
        private readonly Timer _whynot;

        protected DbContext(string conStr)
        {
            _conStr = conStr;
            _whynot = new Timer(_ => Console.WriteLine("Hi"), null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        }

        public void Dispose()
        {
            _whynot?.Dispose();
        }

        public List<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return new List<TEntity>();
        }

        public TEntity Get<TEntity>() where TEntity : class
        {
            return default(TEntity);
        }

        public override string ToString()
        {
            return _conStr;
        }
    }
}
