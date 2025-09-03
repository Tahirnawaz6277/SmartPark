namespace SmartPark.Data.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        void Delete(T entity);
        //Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        //void Update(T entity);
        Task SaveChangesAsync();
    }
}
