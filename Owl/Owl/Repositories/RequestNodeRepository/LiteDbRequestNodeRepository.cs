using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Owl.Contexts;
using Owl.Models;

namespace Owl.Repositories.RequestNodeRepository;

public class LiteDbRequestNodeRepository(IDbContext context) : IRequestNodeRepository
{
    public IEnumerable<RequestNode> GetAll()
    {
        return context.RequestNodes.FindAll();
    }

    public RequestNode Get(Guid id)
    {
        return context.RequestNodes.FindOne((x) => x.Id == id);
    }

    public RequestNode Add(RequestNode entity)
    {
        context.RequestNodes.Insert(entity);
        return entity;
    }

    public RequestNode Update(RequestNode entity)
    {
        context.RequestNodes.Update(entity);
        return entity;
    }
    
    public bool Delete(Guid id)
    {
        Console.WriteLine("Found document by id (res: {0}) with id: {1} ", context.RequestNodes.FindById(id), id);
        Console.WriteLine("Found document by predicate (res: {0}) with id: {1} ", context.RequestNodes.FindOne((x) => x.Id == id), id);
        return context.RequestNodes.Delete(id);
    }

    public RequestNode Upsert(RequestNode entity)
    {
        context.RequestNodes.Upsert(entity);
        return entity;
    }

    public Task<IEnumerable<RequestNode>> GetAllAsync()
    {
        return Task.FromResult(context.RequestNodes.FindAll());
    }

    public Task<RequestNode> GetAsync(Guid id)
    {
        return Task.FromResult(context.RequestNodes.FindOne((x) => x.Id == id));
    }

    public Task<RequestNode> AddAsync(RequestNode entity)
    {
        context.RequestNodes.Insert(entity);
        return Task.FromResult(entity);        
    }

    public Task<RequestNode> UpdateAsync(RequestNode entity)
    {
        context.RequestNodes.Update(entity);
        return Task.FromResult(entity);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(context.RequestNodes.Delete(id));
    }
}