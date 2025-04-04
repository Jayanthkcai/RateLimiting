using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Register Ocelot services
builder.Services.AddOcelot();

var app = builder.Build();

app.UseRouting();

// Use Ocelot Middleware
app.UseOcelot().Wait();

app.Run();
