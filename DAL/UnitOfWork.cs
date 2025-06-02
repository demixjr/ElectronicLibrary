using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

    namespace DAL
    {
        public class UnitOfWork : IUnitOfWork
        {
            private DbContext _context;

            private Dictionary<Type, object> _repositories;

            private IDbContextTransaction _transaction;


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
        public void BeginTransaction()
        {
            if (_transaction == null)
                _transaction = _context.Database.BeginTransaction();
        }
        public void Commit()
        {
            if (_transaction != null)
                _transaction.Commit();

            _transaction.Dispose();
            _transaction = null;
        }
        public void Rollback()
        {
            if (_transaction != null)
            {
                try
                {
                    _transaction.Rollback();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Rollback failed: " + ex.Message);
                }
                finally
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
        }



        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
            _context.Dispose();
            _disposed = true;
        }
    }
    }
