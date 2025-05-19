using System.Linq.Expressions;

namespace DAL
{
    public interface IRepository<T> where T : class
    {
        T Get(int id);
        void Add(T entity);
        void Update(T entity);
        T Find(Expression<Func<T, bool>> predicate);
        void Remove(T entity);
        IEnumerable<T> FindAll(Func<T, bool> predicate);
        IEnumerable<T> GetAll();
    }

}