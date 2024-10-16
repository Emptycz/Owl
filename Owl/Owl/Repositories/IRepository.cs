using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Owl.Repositories;

public interface IRepository<in TType, TResult>
{
    IEnumerable<TResult> GetAll();
    TResult Get(Guid id);
    TResult Add(TType entity);
    TResult Update(TType entity);
    bool Delete(Guid id);
    TResult Upsert(TType entity);

    Task<IEnumerable<TResult>> GetAllAsync();
    Task<TResult> GetAsync(Guid id);
    Task<TResult> AddAsync(TType entity);
    Task<TResult> UpdateAsync(TType entity);
    Task<bool> DeleteAsync(Guid id);
}