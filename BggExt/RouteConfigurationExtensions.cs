namespace BggExt;

public static class ApiConfigurationExtensions
{
    internal const string VersionedApiBasePath = "/api/v{version:apiVersion}/";

    public static RouteHandlerBuilder Get(this WebApplication app, string pattern, Delegate handler)
    {
        return app.MapGet(VersionedApiBasePath.CombinePaths(pattern), handler);
    }

    public static RouteHandlerBuilder Post(this WebApplication app, string pattern, Delegate handler)
    {
        return app.MapPost(VersionedApiBasePath.CombinePaths(pattern), handler);
    }

    public static RouteHandlerBuilder Put(this WebApplication app, string pattern, Delegate handler)
    {
        return app.MapPut(VersionedApiBasePath.CombinePaths(pattern), handler);
    }

    public static RouteHandlerBuilder Delete(this WebApplication app, string pattern, Delegate handler)
    {
        return app.MapDelete(VersionedApiBasePath.CombinePaths(pattern), handler);
    }

    public static RouteHandlerBuilder Patch(this WebApplication app, string pattern, Delegate handler)
    {
        return app.MapMethods(VersionedApiBasePath.CombinePaths(pattern), new[] { "PATCH" }, handler);
    }

    public static RouteHandlerBuilder Options(this WebApplication app, string pattern, Delegate handler)
    {
        return app.MapMethods(VersionedApiBasePath.CombinePaths(pattern), new[] { "OPTIONS" }, handler);
    }

    public static RouteHandlerBuilder Head(this WebApplication app, string pattern, Delegate handler)
    {
        return app.MapMethods(VersionedApiBasePath.CombinePaths(pattern), new[] { "HEAD" }, handler);
    }

    internal static string CombinePaths(this string basePath, string path)
    {
        if (string.IsNullOrEmpty(basePath))
            return path;

        if (string.IsNullOrEmpty(path))
            return basePath;

        return basePath.TrimEnd('/') + "/" + path.TrimStart('/');
    }
}
