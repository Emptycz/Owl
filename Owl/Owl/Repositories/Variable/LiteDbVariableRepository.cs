using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Avalonia.Controls;
using Owl.Contexts;
using Owl.Enums;
using Owl.EventModels;
using Owl.Models.Variables;

namespace Owl.Repositories.Variable;

public class LiteDbVariableRepository : IVariableRepository
{
    public event EventHandler<RepositoryEventObject<IVariable>>? RepositoryHasChanged;

    private readonly IDbContext _context;
    public LiteDbVariableRepository(IDbContext context)
    {
        _context = context;

        // FIXME: This need to be redesigned to be more efficient + it feels weird to just pass the event through
        _context.DbContextHasChanged += (sender, operation) =>
            RepositoryHasChanged?.Invoke(this, new RepositoryEventObject<IVariable>(RepositoryEventOperation.SourceChanged));
    }

    public IEnumerable<IVariable> GetAll()
    {
        return _context.GlobalVariables.FindAll();
    }

    public IEnumerable<IVariable> Find(Expression<Func<IVariable, bool>> predicate)
    {
        return _context.GlobalVariables.Find(predicate);
    }

    public IVariable Get(Guid id)
    {
        return _context.GlobalVariables.FindById(id);
    }

    public IVariable Add(IVariable entity)
    {
        _context.GlobalVariables.Insert(entity);
        NotifyChange(entity, RepositoryEventOperation.AddedSingle);
        return entity;
    }
    
    public IEnumerable<IVariable> Add(IEnumerable<IVariable> entity)
    {
        var enumerable = entity as IVariable[] ?? entity.ToArray();
        _context.GlobalVariables.Insert(enumerable);
        NotifyChange(RepositoryEventOperation.AddedMany);
        return enumerable;
    }

    public IVariable Update(IVariable entity)
    {
        _context.GlobalVariables.Update(entity);
        NotifyChange(entity, RepositoryEventOperation.UpdatedSingle);
        return entity;
    }

    public bool Remove(Guid id)
    {
        bool res = _context.GlobalVariables.Delete(id);
        if (!res) return false;

        NotifyChange(new VariableBase{ Id = id }, RepositoryEventOperation.RemovedSingle);
        return true;
    }

    public int DeleteAll()
    {
        int res = _context.GlobalVariables.DeleteAll();

        NotifyChange(new VariableBase{ Id = Guid.Empty }, RepositoryEventOperation.RemovedSingle);
        return res;
    }

    public IVariable Upsert(IVariable entity)
    {
        _context.GlobalVariables.Upsert(entity);
        NotifyChange(entity, RepositoryEventOperation.AddedSingle);
        return entity;
    }

    private void NotifyChange(IVariable entity, RepositoryEventOperation operation)
    {
        RepositoryHasChanged?.Invoke(this, new RepositoryEventObject<IVariable>(entity, operation));
    }

    private void NotifyChange(RepositoryEventOperation operation)
    {
        RepositoryHasChanged?.Invoke(this, new RepositoryEventObject<IVariable>(operation));
    }
}
