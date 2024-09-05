using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Owl.Contexts;

namespace Owl.Repositories.RequestNode;

public class LiteDbRequestNodeRepository(IDbContext context) : IRequestNodeRepository
{
    public event EventHandler<Models.RequestNode>? RepositoryHasChanged;

    public IEnumerable<Models.RequestNode> GetAll()
    {
        return context.RequestNodes.FindAll();
    }

    public IEnumerable<Models.RequestNode> Find(Expression<Func<Models.RequestNode, bool>> predicate)
    {
        return context.RequestNodes.Find(predicate);
    }

    public Models.RequestNode Get(Guid id)
    {
        return context.RequestNodes.FindOne((x) => x.Id == id);
    }

    public Models.RequestNode Add(Models.RequestNode entity)
    {
        context.RequestNodes.Insert(entity);
        NotifyChange(entity);
        return entity;
    }

    public Models.RequestNode Update(Models.RequestNode entity)
    {
        context.RequestNodes.Update(entity);
        NotifyChange(entity);
        return entity;
    }

    public bool Delete(Guid id)
    {
        bool res = context.RequestNodes.Delete(id);
        if (!res) return false;

        NotifyChange( new Models.RequestNode { Id = id, Name = "Deleted Node" });
        return true;
    }

    public int DeleteAll()
    {
        int res = context.RequestNodes.DeleteAll();

        NotifyChange(new Models.RequestNode { Id = Guid.Empty, Name = "Deleted All Nodes" });
        return res;
    }

    public Models.RequestNode Upsert(Models.RequestNode entity)
    {
        context.RequestNodes.Upsert(entity);
        NotifyChange(entity);
        return entity;
    }

    private void NotifyChange(Models.RequestNode entity)
    {
        RepositoryHasChanged?.Invoke(this, entity);
    }
}
