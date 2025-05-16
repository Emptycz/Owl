using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using Owl.Contexts;
using Owl.Enums;
using Owl.EventModels;
using Owl.Models.Requests;
using Serilog;

namespace Owl.Repositories.RequestNode;

public class LiteDbRequestNodeRepository : IRequestNodeRepository
{
    public event EventHandler<RepositoryEventObject<IRequest>>? RepositoryHasChanged;

    private readonly IDbContext _context;
    public LiteDbRequestNodeRepository(IDbContext context)
    {
        _context = context;
        // FIXME: This need to be redesigned to be more efficient + it feels weird to just pass the event through
        _context.DbContextHasChanged += (sender, operation) =>
            RepositoryHasChanged?.Invoke(this, new RepositoryEventObject<IRequest>(RepositoryEventOperation.SourceChanged));
    }

    public IEnumerable<IRequest> GetAll()
    {
        return _context.RequestNodes.FindAll();
    }

    public IEnumerable<IRequest> Find(Expression<Func<IRequest, bool>> predicate)
    {
        return _context.RequestNodes.Find(predicate);
    }

    public IRequest Get(Guid id)
    {
        return _context.RequestNodes.FindOne((x) => x.Id == id);
    }

    public IRequest Add(IRequest entity)
    {
        _context.RequestNodes.Insert(entity);
        NotifyChange(entity, RepositoryEventOperation.AddedSingle);
        return entity;
    }

    public IEnumerable<IRequest> Add(IEnumerable<IRequest> entity)
    {
        var enumerable = entity as IRequest[] ?? entity.ToArray();

        _context.RequestNodes.Insert(enumerable);
        NotifyChange(RepositoryEventOperation.AddedMany);
        return enumerable;
    }

    public IRequest Update(IRequest entity)
    {
        Log.Debug($"Updating entity: {JsonSerializer.Serialize(entity, typeof(object))}");
        _context.RequestNodes.Update(entity);
        NotifyChange(entity, RepositoryEventOperation.UpdatedSingle);
        return entity;
    }

    public bool Remove(Guid id)
    {
        var oldValue = _context.RequestNodes.FindOne(x => x.Id == id);
        bool res = _context.RequestNodes.Delete(id);
        if (!res) return false;

        NotifyChange(oldValue, RepositoryEventOperation.RemovedSingle);
        return true;
    }

    public int DeleteAll()
    {
        int res = _context.RequestNodes.DeleteAll();

        NotifyChange(new RequestBase { Id = Guid.Empty, Name = "Deleted All Nodes" }, RepositoryEventOperation.RemovedSingle);
        return res;
    }

    public IRequest Upsert(IRequest entity)
    {
        _context.RequestNodes.Upsert(entity);
        NotifyChange(entity, RepositoryEventOperation.AddedSingle);
        return entity;
    }

    private void NotifyChange(IRequest entity, RepositoryEventOperation operation)
    {
        RepositoryHasChanged?.Invoke(this, new RepositoryEventObject<IRequest>(entity, operation));
    }

    private void NotifyChange(RepositoryEventOperation operation)
    {
        RepositoryHasChanged?.Invoke(this, new RepositoryEventObject<IRequest>(operation));
    }
}
