using System.Threading.RateLimiting;
using IPChecker.Domain;
using IPChecker.Domain.Options;
using IpStack.Extensions;
using Microsoft.AspNetCore.RateLimiting;
using ZiggyCreatures.Caching.Fusion;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 40;
        options.Window = TimeSpan.FromSeconds(12);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 20;
    }));

builder.Services.Scan(s => s.FromCallingAssembly()
    .AddClasses(c => c.AssignableTo<IService>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.Configure<CacheOptions>(
    builder.Configuration.GetSection(CacheOptions.Name));

builder.Services.AddFusionCache()
    .WithDefaultEntryOptions(new FusionCacheEntryOptions
    {
        Duration = TimeSpan.FromMinutes(1)
    });
builder.Services.AddIpStack("");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();