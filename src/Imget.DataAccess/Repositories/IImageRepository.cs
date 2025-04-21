namespace Imget.DataAccess.Repositories;

public interface IImageRepository
{
    Task<Stream> GetAsync(string id, CancellationToken cancellationToken = default);
    Task SaveAsync(string id, Stream stream, CancellationToken cancellationToken = default);
}