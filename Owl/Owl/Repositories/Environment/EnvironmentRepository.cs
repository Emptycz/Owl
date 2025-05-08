using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Owl.Contexts;
using Owl.Enums;
using Owl.EventModels;

namespace Owl.Repositories.Environment;

public class EnvironmentRepository : IEnvironmentRepository
{
    private readonly IDbContext _context;

    public EnvironmentRepository(IDbContext context)
    {
        _context = context;
    }

    public event EventHandler<RepositoryEventObject<Models.Environment>>? RepositoryHasChanged;
    public IEnumerable<Models.Environment> GetAll()
    {
        return _context.Environments.FindAll();
    }

    public IEnumerable<Models.Environment> Find(Expression<Func<Models.Environment, bool>> predicate)
    {
        return _context.Environments.Find(predicate);
    }

    public Models.Environment? Get(Guid id)
    {
        return _context.Environments.FindById(id);
    }

    public Models.Environment Add(Models.Environment entity)
    {
        _context.Environments.Insert(entity);
        NotifyChange(entity, RepositoryEventOperation.AddedOne);
        return entity;
    }

    public IEnumerable<Models.Environment> Add(IEnumerable<Models.Environment> entity)
    {
        var environments = entity as Models.Environment[] ?? entity.ToArray();
        _context.Environments.Insert(environments);
        NotifyChange(RepositoryEventOperation.AddedMultiple);
        return environments;
    }

    public Models.Environment Update(Models.Environment entity)
    {
        _context.Environments.Update(entity);
        NotifyChange(entity, RepositoryEventOperation.UpdatedOne);
        return entity;
    }

    public bool Remove(Guid id)
    {
        var entity = Get(id);
        if (entity is null)
        {
            return false;
        }
        _context.Environments.Delete(id);
        NotifyChange(entity, RepositoryEventOperation.RemovedOne);
        return true;
    }

    public int DeleteAll()
    {
        int res = _context.Environments.DeleteAll();
        return res;
    }

    public Models.Environment Upsert(Models.Environment entity)
    {
        return entity.Id == Guid.Empty ? Add(entity) : Update(entity);
    }

    private void NotifyChange(Models.Environment model, RepositoryEventOperation operation)
    {
        RepositoryHasChanged?.Invoke(this, new RepositoryEventObject<Models.Environment>(model, operation));
    }

    private void NotifyChange(RepositoryEventOperation operation)
    {
        RepositoryHasChanged?.Invoke(this, new RepositoryEventObject<Models.Environment>(operation));
    }
}
