using Heinekamp.CodingChallenge.FileApi.Common.Models;

namespace Heinekamp.CodingChallenge.FileApi.Persistence;

public interface IRepository<T> where T : class
{
    Task CreateAsync(T item, CancellationToken cancellationToken);
    Task UpdateAsync(T item, CancellationToken cancellationToken);
    
    Task<T?> GetAsync(Guid id, string currentUser, CancellationToken cancellationToken);

    Task<List<T>> GetAsync(SearchParams searchParams, CancellationToken cancellationToken);
}