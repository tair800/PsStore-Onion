using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PsStore.Application.Interfaces.Repositories;
using PsStore.Domain.Common;
using System.Linq.Expressions;

namespace PsStore.Infrastructure.Repositories
{
    public class ReadRepository<T> : IReadRepository<T> where T : class, IEntityBase, new()
    {
        private readonly DbContext _dbContext;
        private DbSet<T> Table => _dbContext.Set<T>();

        public ReadRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool enableTracking = false, bool includeDeleted = false)
        {
            IQueryable<T> queryable = Table.AsQueryable();

            if (!includeDeleted)
            {
                queryable = queryable.Where(e => !e.IsDeleted);
            }

            if (!enableTracking)
                queryable = queryable.AsNoTracking();

            if (include != null)
                queryable = include(queryable);

            if (predicate != null)
                queryable = queryable.Where(predicate);

            if (orderBy != null)
                return await orderBy(queryable).ToListAsync();

            return await queryable.ToListAsync();
        }

        public async Task<IList<T>> GetAllByPagingAsync(Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool enableTracking = false, bool includeDeleted = false, int currentPage = 1, int pageSize = 3)
        {
            IQueryable<T> queryable = Table.AsQueryable();

            if (!includeDeleted)
            {
                queryable = queryable.Where(e => !e.IsDeleted);
            }

            if (!enableTracking)
                queryable = queryable.AsNoTracking();

            if (include != null)
                queryable = include(queryable);

            if (predicate != null)
                queryable = queryable.Where(predicate);

            if (orderBy != null)
                queryable = orderBy(queryable);

            return await queryable.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool enableTracking = false, bool includeDeleted = false)
        {
            IQueryable<T> queryable = Table.AsQueryable();

            if (!includeDeleted)
            {
                queryable = queryable.Where(e => !e.IsDeleted);
            }

            if (!enableTracking)
                queryable = queryable.AsNoTracking();

            if (include != null)
                queryable = include(queryable);

            return await queryable.FirstOrDefaultAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, bool includeDeleted = false)
        {
            IQueryable<T> queryable = Table.AsQueryable();

            if (!includeDeleted)
            {
                queryable = queryable.Where(e => !e.IsDeleted);
            }

            if (predicate != null)
                queryable = queryable.Where(predicate);

            return await queryable.CountAsync();
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate, bool enableTracking = false, bool includeDeleted = false)
        {
            IQueryable<T> queryable = Table.AsQueryable();

            if (!includeDeleted)
            {
                queryable = queryable.Where(e => !e.IsDeleted);
            }

            if (!enableTracking)
                queryable = queryable.AsNoTracking();

            return queryable.Where(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false)
        {
            IQueryable<T> queryable = Table.AsQueryable();

            if (!includeDeleted)
            {
                queryable = queryable.Where(e => !e.IsDeleted);
            }

            return await queryable.AnyAsync(predicate);
        }
    }
}
