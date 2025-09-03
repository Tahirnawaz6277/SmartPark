using Microsoft.EntityFrameworkCore;
using SmartPark.Data.Contexts;
using SmartPark.Data.Repositories.Interfaces;

namespace SmartPark.Data.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ParkingDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ParkingDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
           await _dbSet.AddAsync(entity);
            return entity;
        }

        public void Delete(T entity)
        {
             _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await  _dbSet.FindAsync(id); 
        }

        public async Task SaveChangesAsync()
        {
           await _context.SaveChangesAsync();
        }

        //public async Task<T> Update<T,TId>(T entity)
        //{
        //    //await _dbSet.Update(entity);
        //    T entityInDb = await _dbSet.FindAsync(entity);
        //    if (entityInDb != null)
        //    {
        //        _context.Entry(entityInDb).CurrentValues.SetValues(entity);
        //        return entity;
        //    }
        //    else
        //    {
        //        throw new Exception("Not Found");
        //    }
        //}


    }
}
