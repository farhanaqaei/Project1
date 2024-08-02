﻿using Microsoft.EntityFrameworkCore;
using Project1.Core.General.Interfaces;

namespace Project1.Infrastructure.Data;

public class GenericRepository<T>: IGenericRepository<T> where T : class, IBaseEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task AddEntity(T entity)
    {
        //entity.CreateDate = DateTime.Now;
        //entity.LastUpdateDate = entity.CreateDate;
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeEntities(List<T> entities)
    {
        foreach (var entity in entities)
        {
            await AddEntity(entity);
        }
    }

    public async Task<T> GetEntityById(long entityId)
    {
        //return await _dbSet.SingleOrDefaultAsync(x => x.Id == entityId && x.IsDeleted == false);
        return await _dbSet.SingleOrDefaultAsync(x => x.Id == entityId);
    }

    public IQueryable<T> GetQuery()
    {
        return _dbSet.AsQueryable();
    }

    public void EditEntity(T entity)
    {
        //ntity.LastUpdateDate = DateTime.Now;
        _dbSet.Update(entity);
    }

    public void DeleteEntity(T entity)
    {
        //entity.LastUpdateDate = DateTime.Now;
        //entity.IsDeleted = true;
        EditEntity(entity);
    }

    public async Task DeleteEntity(long entityId)
    {
        T entity = await GetEntityById(entityId);
        if (entity != null) DeleteEntity(entity);
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_context != null)
        {
            await _context.DisposeAsync();
        }
    }
}
