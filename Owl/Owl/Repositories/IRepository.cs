using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Owl.Repositories;

public interface IRepository<in TType, TResult>
{
    // TODO: Design this properly, maybe we should not pass the boolean (forceRefetch) but the changed entity instead
    event EventHandler<TResult> RepositoryHasChanged;

    IEnumerable<TResult> GetAll();
    IEnumerable<TResult> Find(Expression<Func<TResult, bool>> predicate);
    TResult? Get(Guid id);
    TResult Add(TType entity);
    TResult Update(TType entity);
    bool Delete(Guid id);
    int DeleteAll();
    TResult Upsert(TType entity);

    Task<IEnumerable<TResult>> GetAllAsync();
    Task<TResult?> GetAsync(Guid id);
    Task<TResult> AddAsync(TType entity);
    Task<TResult> UpdateAsync(TType entity);
    Task<bool> DeleteAsync(Guid id);
}
