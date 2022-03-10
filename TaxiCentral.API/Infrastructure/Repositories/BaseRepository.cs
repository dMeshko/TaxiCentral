using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TaxiCentral.API.Infrastructure.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> GetAll();
        Task<int> Count();
        Task<T?> GetSingle(Guid id);
        Task<T?> GetSingle(Expression<Func<T, bool>> predicate);
        Task<T?> GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<List<T>> FindBy(Expression<Func<T, bool>> predicate);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task DeleteWhere(Expression<Func<T, bool>> predicate);
        Task Commit();
    }

    public class BaseRepository<T> : IBaseRepository<T> where T: class
    {
        private readonly TaxiCentralContext _context;
        public DbSet<T> Query { get; }

        public BaseRepository(TaxiCentralContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Query = context.Set<T>();
        }

        public Task<List<T>> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            return includeProperties.Aggregate(
                    query,
                    (current, includeProperty) => current.Include(includeProperty))
                .ToListAsync();
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public Task<int> Count()
        {
            return _context.Set<T>().CountAsync();
        }

        public Task<T?> GetSingle(Guid id)
        {
            return _context.Set<T>().FindAsync(id).AsTask();
        }

        public Task<T?> GetSingle(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public Task<T?> GetSingle(Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.Where(predicate).FirstOrDefaultAsync();
        }

        public Task<List<T>> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate).ToListAsync();
        }

        public Task Add(T entity)
        {
            _context.Entry(entity);
            _context.Set<T>().AddAsync(entity);
            return Task.CompletedTask;
        }

        public Task Update(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public Task Delete(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry(entity);
            dbEntityEntry.State = EntityState.Deleted;
            return Task.CompletedTask;
        }

        public Task DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(predicate);

            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Deleted;
            }

            return Task.CompletedTask;
        }

        public Task Commit()
        {
            return _context.SaveChangesAsync();
        }
    }
}
