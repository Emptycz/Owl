using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;
using Owl.Contexts;
using Owl.Enums;
using Owl.EventModels;
using Owl.Models.Requests;
using Serilog;

namespace Owl.Repositories.RequestNode;

public class LiteDbRequestNodeRepository(IDbContext context) : IRequestNodeRepository
{
    public event EventHandler<RepositoryEventObject<IRequest>>? RepositoryHasChanged;

    public IEnumerable<IRequest> GetAll()
    {
        return context.RequestNodes.FindAll();
    }

    public IEnumerable<IRequest> Find(Expression<Func<IRequest, bool>> predicate)
    {
        return context.RequestNodes.Find(predicate);
    }

    public IRequest Get(Guid id)
    {
        return context.RequestNodes.FindOne((x) => x.Id == id);
    }

    public IRequest Add(IRequest entity)
    {
        context.RequestNodes.Insert(entity);
        NotifyChange(entity, RepositoryEventOperation.Add);
        return entity;
    }

    public IRequest Update(IRequest entity)
    {
        Log.Debug($"Updating entity: {JsonSerializer.Serialize(entity, typeof(object))}");
        context.RequestNodes.Update(entity);
        NotifyChange(entity, RepositoryEventOperation.Update);
        return entity;
    }

    public bool Remove(Guid id)
    {
        var oldValue = context.RequestNodes.FindOne(x => x.Id == id);
        bool res = context.RequestNodes.Delete(id);
        if (!res) return false;

        NotifyChange(oldValue, RepositoryEventOperation.Remove);
        return true;
    }

    public int DeleteAll()
    {
        int res = context.RequestNodes.DeleteAll();

        NotifyChange(new RequestBase { Id = Guid.Empty, Name = "Deleted All Nodes" }, RepositoryEventOperation.Remove);
        return res;
    }

    public IRequest Upsert(IRequest entity)
    {
        context.RequestNodes.Upsert(entity);
        NotifyChange(entity, RepositoryEventOperation.Add);
        return entity;
    }

    private void NotifyChange(IRequest entity, RepositoryEventOperation operation)
    {
        RepositoryHasChanged?.Invoke(this, new RepositoryEventObject<IRequest>(entity, operation));
    }
}
