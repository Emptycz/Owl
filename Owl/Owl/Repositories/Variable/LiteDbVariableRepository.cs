using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using Owl.Contexts;
using Owl.Models;

namespace Owl.Repositories.Variable;

public class LiteDbVariableRepository(IDbContext context) : IVariableRepository
{
    public event EventHandler<VariableBase>? RepositoryHasChanged;

    public IEnumerable<VariableBase> GetAll()
    {
        var res = context.GlobalVariables.FindAll();
        foreach (var v in res)
        {
            Console.WriteLine(JsonSerializer.Serialize(v));
        }
        return res;
    }

    public IEnumerable<VariableBase> Find(Expression<Func<VariableBase, bool>> predicate)
    {
        return context.GlobalVariables.Find(predicate);
    }

    public VariableBase Get(Guid id)
    {
        return context.GlobalVariables.FindById(id);
    }

    public VariableBase Add(VariableBase entity)
    {
        context.GlobalVariables.Insert(entity);
        NotifyChange(entity);
        return entity;
    }

    public VariableBase Update(VariableBase entity)
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

    public int DeleteAll()
    {
        int res = context.GlobalVariables.DeleteAll();

        NotifyChange(new OwlVariable{ Id = Guid.Empty });
        return res;
    }

    public VariableBase Upsert(VariableBase entity)
    {
        context.GlobalVariables.Upsert(entity);
        NotifyChange(entity);
        return entity;
    }

    public Task<IEnumerable<VariableBase>> GetAllAsync()
    {
        return Task.FromResult(context.GlobalVariables.FindAll());
    }

    public Task<VariableBase?> GetAsync(Guid id)
    {
        return Task.FromResult(context.GlobalVariables.FindById(id));
    }

    public Task<VariableBase> AddAsync(VariableBase entity)
    {
        context.GlobalVariables.Insert(entity);
        NotifyChange(entity);
        return Task.FromResult(entity);
    }

    public Task<VariableBase> UpdateAsync(VariableBase entity)
    {
        context.GlobalVariables.Update(entity);
        NotifyChange(entity);
        return Task.FromResult(entity);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(context.GlobalVariables.Delete(id));
    }

    private void NotifyChange(VariableBase entity)
    {
        RepositoryHasChanged?.Invoke(this, entity);
    }
}
