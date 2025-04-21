namespace Imget.Endpoints;

internal static class ScalarEndpoints
{
    internal static WebApplication MapScalarEndpoints(this WebApplication app)
    {
        app.MapGet("/api", Scalar);

        return app;
    }

    private static Task Scalar(HttpContext ctx)
    {
        ctx.Response.Redirect("/scalar/v1");
        return Task.CompletedTask;
    }
}