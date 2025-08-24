namespace SchnapsSchuss.Models.Databases
{
    public interface IDatabase<T>
    {
        Task<T> GetOneAsync(int id);

        Task<List<T>> GetAllAsync();

        Task<int> SaveAsync(T entity);

        Task<int> DeleteAsync(T entity);
    }
}
