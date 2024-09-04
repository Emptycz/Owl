using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Owl.Contexts;

namespace Owl.Repositories.Settings;

public class SettingsRepository : ISettingsRepository
{
    public Models.Settings Current { get; set; }
    public event EventHandler<Models.Settings>? RepositoryHasChanged;

    private readonly IDbContext _context;

    public SettingsRepository(IDbContext context)
    {
        _context = context;
        Current = GetAll().LastOrDefault() ?? new Models.Settings();
    }

    public IEnumerable<Models.Settings> GetAll()
    {
        return _context.Settings.FindAll();
    }

    public IEnumerable<Models.Settings> Find(Expression<Func<Models.Settings, bool>> predicate)
    {
        return _context.Settings.Find(predicate);
    }

    public Models.Settings? Get(Guid id)
    {
        return _context.Settings.FindById(id);
    }

    public Models.Settings Add(Models.Settings entity)
    {
        _context.Settings.Insert(entity);
        RepositoryHasChanged?.Invoke(this, entity);
        Current = entity;
        return entity;
    }

    public Models.Settings Update(Models.Settings entity)
    {
        _context.Settings.Update(entity);
        RepositoryHasChanged?.Invoke(this, entity);
        Current = entity;
        return entity;
    }

    public bool Delete(Guid id)
    {
        return _context.Settings.Delete(id);
    }

    public Models.Settings Upsert(Models.Settings entity)
    {
        return _context.Settings.FindById(entity.Id) is not null ? Update(entity) : Add(entity);
    }

    public Task<IEnumerable<Models.Settings>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Models.Settings?> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Models.Settings> AddAsync(Models.Settings entity)
    {
        throw new NotImplementedException();
    }

    public Task<Models.Settings> UpdateAsync(Models.Settings entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
