using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Owl.Contexts;
using Owl.Models.Variables;

namespace Owl.Repositories.Variable;

public class LiteDbVariableRepository(IDbContext context) : IVariableRepository
{
    public event EventHandler<IVariable>? RepositoryHasChanged;

    public IEnumerable<IVariable> GetAll()
    {
        return context.GlobalVariables.FindAll();
    }

    public IEnumerable<IVariable> Find(Expression<Func<IVariable, bool>> predicate)
    {
        return context.GlobalVariables.Find(predicate);
    }

    public IVariable Get(Guid id)
    {
        return context.GlobalVariables.FindById(id);
    }

    public IVariable Add(IVariable entity)
    {
        context.GlobalVariables.Insert(entity);
        NotifyChange(entity);
        return entity;
    }

    public IVariable Update(IVariable entity)
    {
        context.GlobalVariables.Update(entity);
        NotifyChange(entity);
        return entity;
    }

    public bool Delete(Guid id)
    {
        bool res = context.GlobalVariables.Delete(id);
        if (!res) return false;

        NotifyChange(new VariableBase{ Id = id });
        return true;
    }

    public int DeleteAll()
    {
        int res = context.GlobalVariables.DeleteAll();

        NotifyChange(new VariableBase{ Id = Guid.Empty });
        return res;
    }

    public IVariable Upsert(IVariable entity)
    {
        context.GlobalVariables.Upsert(entity);
        NotifyChange(entity);
        return entity;
    }

    private void NotifyChange(IVariable entity)
    {
        RepositoryHasChanged?.Invoke(this, entity);
    }
}
