using Microsoft.EntityFrameworkCore.Storage;
using SmartPark.Data.Contexts;
using SmartPark.Data.Repositories.Interfaces;
using System.Collections;

namespace SmartPark.Data.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ParkingDbContext _context;
        private Hashtable _repositories;

        public UnitOfWork(ParkingDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            _repositories ??= new Hashtable();
            var type = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var respositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
                _repositories.Add(type, respositoryInstance);
            }
            return (IGenericRepository<TEntity>)_repositories[type];

        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction != null)
            {
                await transaction.CommitAsync();
            }
        }

        public async Task RollbackTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync();
            }
        }


        public async void Dispose()
        {
            _context.Dispose();
        }
        
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }


    }
}
