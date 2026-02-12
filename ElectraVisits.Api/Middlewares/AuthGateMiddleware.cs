using Microsoft.AspNetCore.Authorization;

namespace ElectraVisits.Api.Middlewares;

public class AuthGateMiddleware
{
    private readonly RequestDelegate _next;

    public AuthGateMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext ctx)
    {
        var endpoint = ctx.GetEndpoint();
        var allowAnon = endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is not null;

        if (!allowAnon)
        {
            if (ctx.User?.Identity?.IsAuthenticated != true)
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await ctx.Response.WriteAsync("Unauthorized");
                return;
            }
        }

        await _next(ctx);
    }
}