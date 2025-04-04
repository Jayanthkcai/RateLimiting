using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.RateLimiting;

namespace OnlineBanking.Config
{
    public class RateLimitingConfig
    {
        public static void ConfigureRateLimiting(IServiceCollection services)
        {
            // Different rate limiting strategies for various microservices
            services.AddRateLimiter(options =>
            {
                // Global Rate Limiting Policy
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 100, // 100 requests per window
                            QueueLimit = 20,   // Additional 20 requests can be queued
                            Window = TimeSpan.FromSeconds(1) // 1-second window
                        });
                });

                // Specific Policies for Different Endpoints
                options.AddPolicy("Fixed", context =>
                {
                    // Stricter limits for transaction endpoints
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,  // 50 transactions per second
                            Window = TimeSpan.FromSeconds(10),
                            QueueLimit = 3,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("Token", context =>
                {
                    // More restrictive for fraud detection
                    return RateLimitPartition.GetTokenBucketLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                        factory: partition => new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = 30,   // 30 tokens
                            QueueLimit = 0,    // 5 additional queued requests
                            ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                            TokensPerPeriod = 10, // 10 tokens replenished per second
                            AutoReplenishment = false
                        });
                });

                // Notification Service Policy
                options.AddPolicy("Sliding", context =>
                {
                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                        factory: partition => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 20,  // 20 notifications per window
                            Window = TimeSpan.FromSeconds(1),
                            SegmentsPerWindow = 4,
                            QueueLimit = 5
                        });
                });

                // Customized Error Handling
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsync("Too many requests, please try again later.", cancellationToken);
                };
            });
        }
    }
}
