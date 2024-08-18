using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

    public Models.RequestNode Upsert(Models.RequestNode entity)
    {
        context.RequestNodes.Upsert(entity);
        NotifyChange(entity);
        return entity;
    }

    public Task<IEnumerable<Models.RequestNode>> GetAllAsync()
    {
        return Task.FromResult(context.RequestNodes.FindAll());
    }

    public Task<Models.RequestNode> GetAsync(Guid id)
    {
        return Task.FromResult(context.RequestNodes.FindOne((x) => x.Id == id));
    }

    public Task<Models.RequestNode> AddAsync(Models.RequestNode entity)
    {
        context.RequestNodes.Insert(entity);
        NotifyChange(entity);
        return Task.FromResult(entity);
    }

    public Task<Models.RequestNode> UpdateAsync(Models.RequestNode entity)
    {
        context.RequestNodes.Update(entity);
        NotifyChange(entity);
        return Task.FromResult(entity);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(context.RequestNodes.Delete(id));
    }

    private void NotifyChange(Models.RequestNode entity)
    {
        RepositoryHasChanged?.Invoke(this, entity);
    }
}
