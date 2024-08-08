namespace Project1.Core.Generals.Interfaces;

public interface IGenericRepository<T> : IDisposable, IAsyncDisposable where T : IBaseEntity
{
    Task AddEntity(T entity);
    Task AddRangeEntities(List<T> entities);
    Task<T> GetEntityById(long entityId);
    IQueryable<T> GetQuery();
    void EditEntity(T entity);
    void DeleteEntity(T entity);
    Task DeleteEntity(long entityId);
    Task SaveChanges();
}
