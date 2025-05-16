using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Owl.Contexts;
using Owl.Enums;
using Owl.EventModels;

namespace Owl.Repositories.Settings;

public class SettingsRepository : ISettingsRepository
{
    public Models.Settings Current { get; set; }
    public event EventHandler<RepositoryEventObject<Models.Settings>>? RepositoryHasChanged;

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
        NotifyChange(entity, RepositoryEventOperation.AddedSingle);
        Current = entity;
        return entity;
    }

    public IEnumerable<Models.Settings> Add(IEnumerable<Models.Settings> entity)
    {
        var settingsEnumerable = entity as Models.Settings[] ?? entity.ToArray();
        _context.Settings.Insert(settingsEnumerable);
        NotifyChange(RepositoryEventOperation.AddedMany);
        return settingsEnumerable;
    }

    public Models.Settings Update(Models.Settings entity)
    {
        _context.Settings.Update(entity);
        NotifyChange(entity, RepositoryEventOperation.UpdatedSingle);
        Current = entity;
        return entity;
    }

    public bool Remove(Guid id)
    {
        return _context.Settings.Delete(id);
    }

    public int DeleteAll()
    {
        return _context.Settings.DeleteAll();
    }

    public Models.Settings Upsert(Models.Settings entity)
    {
        return _context.Settings.FindById(entity.Id) is not null ? Update(entity) : Add(entity);
    }

    private void NotifyChange(Models.Settings model, RepositoryEventOperation operation)
    {
        RepositoryHasChanged?.Invoke(this, new RepositoryEventObject<Models.Settings>(model, operation));
    }

    private void NotifyChange(RepositoryEventOperation operation)
    {
        RepositoryHasChanged?.Invoke(this, new RepositoryEventObject<Models.Settings>(operation));
    }
}
