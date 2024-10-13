using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Owl.Contexts;
using Owl.Models.Requests;

namespace Owl.Repositories.RequestNode;

public class LiteDbRequestNodeRepository(IDbContext context) : IRequestNodeRepository
{
    public event EventHandler<IRequest>? RepositoryHasChanged;

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
        NotifyChange(entity);
        return entity;
    }

    public IRequest Update(IRequest entity)
    {
        context.RequestNodes.Update(entity);
        NotifyChange(entity);
        return entity;
    }

    public bool Delete(Guid id)
    {
        bool res = context.RequestNodes.Delete(id);
        if (!res) return false;

        NotifyChange(new RequestBase { Id = id, Name = "Deleted Node" });
        return true;
    }

    public int DeleteAll()
    {
        int res = context.RequestNodes.DeleteAll();

        NotifyChange(new RequestBase { Id = Guid.Empty, Name = "Deleted All Nodes" });
        return res;
    }

    public IRequest Upsert(IRequest entity)
    {
        context.RequestNodes.Upsert(entity);
        NotifyChange(entity);
        return entity;
    }

    private void NotifyChange(IRequest entity)
    {
        RepositoryHasChanged?.Invoke(this, entity);
    }
}
