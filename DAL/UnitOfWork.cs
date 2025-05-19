using Microsoft.EntityFrameworkCore;

    namespace DAL
    {
        public class UnitOfWork : IUnitOfWork
        {
            private DbContext _context;

            private Dictionary<Type, object> _repositories;

            private bool _disposed = false;


            public UnitOfWork(DbContext context)
            {
                _context = context;
                _repositories = new Dictionary<Type, object>();
            }

            public IRepository<T> GetRepository<T>() where T : class
            {
                if (!_repositories.ContainsKey(typeof(T)))
                {
                    Repository<T> repository = new Repository<T>(_context);
                    _repositories.Add(typeof(T), repository);
                }
                return (IRepository<T>)
                    _repositories[typeof(T)];
            }


            public void Save()
            {
                _context.SaveChanges();
            }



            public void Dispose()
            {
                if (!_disposed)
                {
                    _context.Dispose();
                    _disposed = true;
                }
            }
        }
    }
