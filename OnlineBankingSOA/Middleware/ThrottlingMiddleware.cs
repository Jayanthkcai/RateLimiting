using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace OnlineBanking.Middleware
{
    public class ThrottlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ThrottlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Additional custom throttling logic
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            // Check IP-based restrictions
            if (IsBlockedIP(ipAddress))
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Access denied");
                return;
            }

            // Check user-based throttling
            if (HasExceededUserThrottling(context.User))
            {
                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsync("User throttling limit exceeded");
                return;
            }

            await _next(context);
        }

        private bool IsBlockedIP(string ipAddress)
        {
            // Implementation of IP blocking logic
            return false;
        }

        private bool HasExceededUserThrottling(System.Security.Claims.ClaimsPrincipal user)
        {
            // Implementation of user-based throttling
            return false;
        }
    }
}
