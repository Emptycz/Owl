using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Owl.EventModels;

namespace Owl.Repositories;

public interface IRepository<in TType, TResult>
{
    // TODO: Design this properly, maybe we should not pass the boolean (forceRefetch) but the changed entity instead
    event EventHandler<RepositoryEventObject<TResult>> RepositoryHasChanged;

    IEnumerable<TResult> GetAll();
    IEnumerable<TResult> Find(Expression<Func<TResult, bool>> predicate);
    TResult? Get(Guid id);
    TResult Add(TType entity);
    IEnumerable<TResult> Add(IEnumerable<TType> entity);
    TResult Update(TType entity);
    bool Remove(Guid id);
    int DeleteAll();
    TResult Upsert(TType entity);
}
