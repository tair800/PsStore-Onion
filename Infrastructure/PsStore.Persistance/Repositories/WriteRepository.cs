using Microsoft.EntityFrameworkCore;
using PsStore.Application.Interfaces.Repositories;
using PsStore.Domain.Common;
using System.Linq.Expressions;

namespace PsStore.Infrastructure.Repositories
{
    public class WriteRepository<T> : IWriteRepository<T> where T : class, IEntityBase, new()
    {
        private readonly DbContext dbContext;

        public WriteRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private DbSet<T> Table { get => dbContext.Set<T>(); }

        public async Task AddAsync(T entity)
        {
            await Table.AddAsync(entity);
        }

        public async Task AddRangeAsync(IList<T> entities)
        {
            await Table.AddRangeAsync(entities);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var dbEntry = dbContext.Entry(entity);

            // Loop through properties and only mark modified ones
            foreach (var property in dbEntry.Properties)
            {
                if (property.CurrentValue == null || property.CurrentValue.Equals(property.OriginalValue))
                {
                    property.IsModified = false; // Don't modify unchanged properties
                }
                else
                {
                    property.IsModified = true; // Mark only changed fields as modified
                }
            }

            entity.UpdatedDate = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            return entity;
        }


        public async Task HardDeleteAsync(T entity)
        {
            await Task.Run(() => Table.Remove(entity));
        }

        public async Task HardDeleteRangeAsync(IList<T> entities)
        {
            await Task.Run(() => Table.RemoveRange(entities));
        }

        public async Task SoftDeleteAsync(T entity)
        {
            entity.IsDeleted = true;
            entity.UpdatedDate = DateTime.UtcNow;
            Table.Update(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task RestoreAsync(T entity)
        {
            entity.IsDeleted = false;
            entity.UpdatedDate = DateTime.UtcNow;
            Table.Update(entity);
            await dbContext.SaveChangesAsync();
        }

        public void MarkAsModified<TProperty>(T entity, Expression<Func<T, TProperty>> propertyExpression)
        {
            dbContext.Entry(entity).Property(propertyExpression).IsModified = true;
        }

        public string GenerateSqlForUpdate(T entity)
        {
            var sql = dbContext.Entry(entity).DebugView.LongView;
            return sql;
        }

    }
}
