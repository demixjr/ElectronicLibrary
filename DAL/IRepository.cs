using System.Linq.Expressions;

namespace DAL
{
    public interface IRepository<T> where T : class
    {
        T Get(int id, params Expression<Func<T, object>>[] includes);
        void Add(T entity);
        void Update(T entity);
        T Find(Expression<Func<T, bool>> predicate);
        void Remove(T entity);
        IEnumerable<T> FindAll(Func<T, bool> predicate);
        IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes);
    }

}