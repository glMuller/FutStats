using System.Threading.Tasks;

namespace FutStatsAPI.Domain.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<bool> SaveChangesAsync();
    }
}