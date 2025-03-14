using Microsoft.Extensions.Logging;
using PsStore.Application.Interfaces.Repositories;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Infrastructure.Repositories;
using PsStore.Persistance.Context;
using System.Diagnostics;

namespace PsStore.Persistance.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(AppDbContext dbContext, ILogger<UnitOfWork> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async ValueTask DisposeAsync() => await _dbContext.DisposeAsync();

        public int Save()
        {
            _logger.LogInformation("Saving changes to the database at {Time}", DateTime.UtcNow);
            var result = _dbContext.SaveChanges();
            _logger.LogInformation("Database changes saved successfully. {RowsAffected} rows affected.", result);
            return result;
        }

        public async Task<int> SaveAsync()
        {
            _logger.LogInformation("Starting SaveAsync() at {Time}", DateTime.UtcNow);

            var stopwatch = Stopwatch.StartNew();
            try
            {
                int result = await _dbContext.SaveChangesAsync();
                stopwatch.Stop();

                _logger.LogInformation("SaveAsync() completed successfully in {ElapsedMilliseconds} ms. {RowsAffected} rows affected.",
                    stopwatch.ElapsedMilliseconds, result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving changes to the database at {Time}", DateTime.UtcNow);
                throw;
            }
        }

        IReadRepository<T> IUnitOfWork.GetReadRepository<T>() => new ReadRepository<T>(_dbContext);
        IWriteRepository<T> IUnitOfWork.GetWriteRepository<T>() => new WriteRepository<T>(_dbContext);
    }
}
