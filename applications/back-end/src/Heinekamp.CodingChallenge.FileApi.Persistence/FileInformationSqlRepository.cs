using Heinekamp.CodingChallenge.FileApi.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly.Wrap;

namespace Heinekamp.CodingChallenge.FileApi.Persistence;

public class FileInformationSqlRepository(
    FileInformationDataBaseContext databaseContext,
    AsyncPolicyWrap retryPolicy,
    ILogger<FileInformationSqlRepository> logger) : IRepository<FileInformation>
{
    private readonly DbSet<FileInformation> dbSet = databaseContext.Set<FileInformation>();

    public async Task CreateAsync(FileInformation item, CancellationToken cancellationToken)
    {
        try
        {
            await dbSet.AddAsync(item, cancellationToken);
            await databaseContext.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException exception)
        {
            logger.LogError(exception, exception.Message);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            logger.LogError(exception, exception.Message);
        }
    }

    public async Task UpdateAsync(FileInformation item, CancellationToken cancellationToken)
    {
        dbSet.Update(item);
        
        try
        {
            await databaseContext.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException exception)
        {
            logger.LogError(exception, exception.Message);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            logger.LogError(exception, exception.Message);
        }
    }

    public async Task<FileInformation?> GetAsync(Guid id, string currentUser, CancellationToken cancellationToken)
    {
        return await retryPolicy.ExecuteAsync(async () =>
        {
            var item = await dbSet
                .FirstOrDefaultAsync(
                    e => e.FileId == id && e.UploadedBy == currentUser,
                    cancellationToken);
        
            return item;
        });
    }

    public async Task<List<FileInformation>> GetAsync(SearchParams searchParams, CancellationToken cancellationToken)
    {
        return await retryPolicy.ExecuteAsync(async () =>
        {
            var pagedItems = dbSet
                .Where(e => e.UploadedBy == searchParams.CurrentUser)
                .OrderBy(e => e.UploadedOn)
                .Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
                .Take(searchParams.PageSize);

            var result = await pagedItems.ToListAsync(cancellationToken);

            return result;
        });
    }
}