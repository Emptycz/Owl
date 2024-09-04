using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Owl.Contexts;

namespace Owl.Repositories.Environment;

public class EnvironmentRepository : IEnvironmentRepository
{
    private readonly IDbContext _context;

    public EnvironmentRepository(IDbContext context)
    {
        _context = context;
    }

    public event EventHandler<Models.Environment>? RepositoryHasChanged;
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
        RepositoryHasChanged?.Invoke(this, entity);
        return entity;
    }

    public Models.Environment Update(Models.Environment entity)
    {
        _context.Environments.Update(entity);
        RepositoryHasChanged?.Invoke(this, entity);
        return entity;
    }

    public bool Delete(Guid id)
    {
        var entity = Get(id);
        if (entity is null)
        {
            return false;
        }
        _context.Environments.Delete(id);
        RepositoryHasChanged?.Invoke(this, entity);
        return true;
    }

    public int DeleteAll()
    {
        int res = _context.Environments.DeleteAll();
        RepositoryHasChanged?.Invoke(this, new Models.Environment { Id = Guid.Empty });
        return res;
    }

    public Models.Environment Upsert(Models.Environment entity)
    {
        return entity.Id == Guid.Empty ? Add(entity) : Update(entity);
    }

    public Task<IEnumerable<Models.Environment>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Models.Environment?> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Models.Environment> AddAsync(Models.Environment entity)
    {
        throw new NotImplementedException();
    }

    public Task<Models.Environment> UpdateAsync(Models.Environment entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
