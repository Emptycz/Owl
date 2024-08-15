using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Owl.Contexts;
using Owl.Repositories.RequestNodeRepository;

namespace Owl.Repositories.RequestNode;

public class LiteDbRequestNodeRepository(IDbContext context) : IRequestNodeRepository
{
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
        return entity;
    }

    public Models.RequestNode Update(Models.RequestNode entity)
    {
        context.RequestNodes.Update(entity);
        return entity;
    }

    public bool Delete(Guid id)
    {
        return context.RequestNodes.Delete(id);
    }

    public Models.RequestNode Upsert(Models.RequestNode entity)
    {
        context.RequestNodes.Upsert(entity);
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
        return Task.FromResult(entity);
    }

    public Task<Models.RequestNode> UpdateAsync(Models.RequestNode entity)
    {
        context.RequestNodes.Update(entity);
        return Task.FromResult(entity);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(context.RequestNodes.Delete(id));
    }
}
