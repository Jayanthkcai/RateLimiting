using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OnlineBanking.Middleware;
using OnlineBanking.Services;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

// Configure rate limiting
OnlineBanking.Config.RateLimitingConfig.ConfigureRateLimiting(builder.Services);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});
var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "/swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
//}

// Add rate limiter middleware
app.UseRateLimiter();

// Add custom throttling middleware
app.UseMiddleware<ThrottlingMiddleware>();

app.UseAuthorization();
app.MapControllers();
app.Run();
