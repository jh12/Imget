using Imget.DataAccess.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Imget.Endpoints;

internal static class ImageEndpoints
{
    internal static WebApplication MapImageEndpoints(this WebApplication app)
    {
        app.MapGet("/image/{id}", GetImage);

        return app;
    }

    private static async Task<FileStreamHttpResult> GetImage(HttpContext ctx, string id, IImageRepository imageRepository, CancellationToken cancellationToken)
    {
        Stream stream = await imageRepository.GetAsync(id, cancellationToken);

        // TODO: Return correct mime type
        return TypedResults.Stream(stream, "image/png");
    }
}