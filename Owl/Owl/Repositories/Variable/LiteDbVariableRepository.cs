using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Owl.Contexts;
using Owl.Models;

namespace Owl.Repositories.Variable;

public class LiteDbVariableRepository(IDbContext context) : IVariableRepository
{
    public event EventHandler<OwlVariable>? RepositoryHasChanged;

    public IEnumerable<OwlVariable> GetAll()
    {
        return context.GlobalVariables.FindAll();
    }

    public IEnumerable<OwlVariable> Find(Expression<Func<OwlVariable, bool>> predicate)
    {
        return context.GlobalVariables.Find(predicate);
    }

    public OwlVariable Get(Guid id)
    {
        return context.GlobalVariables.FindById(id);
    }

    public OwlVariable Add(OwlVariable entity)
    {
        context.GlobalVariables.Insert(entity);
        NotifyChange(entity);
        return entity;
    }

    public OwlVariable Update(OwlVariable entity)
    {
        context.GlobalVariables.Update(entity);
        NotifyChange(entity);
        return entity;
    }

    public bool Delete(Guid id)
    {
        bool res = context.GlobalVariables.Delete(id);
        if (!res) return false;

        NotifyChange(new OwlVariable{ Id = id });
        return true;
    }

    public OwlVariable Upsert(OwlVariable entity)
    {
        context.GlobalVariables.Upsert(entity);
        NotifyChange(entity);
        return entity;
    }

    public Task<IEnumerable<OwlVariable>> GetAllAsync()
    {
        return Task.FromResult(context.GlobalVariables.FindAll());
    }

    public Task<OwlVariable> GetAsync(Guid id)
    {
        return Task.FromResult(context.GlobalVariables.FindById(id));
    }

    public Task<OwlVariable> AddAsync(OwlVariable entity)
    {
        context.GlobalVariables.Insert(entity);
        NotifyChange(entity);
        return Task.FromResult(entity);
    }

    public Task<OwlVariable> UpdateAsync(OwlVariable entity)
    {
        context.GlobalVariables.Update(entity);
        NotifyChange(entity);
        return Task.FromResult(entity);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(context.GlobalVariables.Delete(id));
    }

    private void NotifyChange(OwlVariable entity)
    {
        RepositoryHasChanged?.Invoke(this, entity);
    }
}
