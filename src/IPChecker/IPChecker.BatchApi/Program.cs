using System.Reflection;
using System.Threading.RateLimiting;
using FluentValidation;
using Hangfire;
using Hangfire.MemoryStorage;
using IPChecker.BatchApi.Policies;
using IPChecker.BatchApi.Services;
using IPChecker.BatchApi.Validators;
using IPChecker.Domain;
using IPChecker.Domain.Options;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient<IBatchIpService, BatchService>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
    .AddPolicyHandler(RetryPolicy.GetRetryPolicy());;

builder.Services.Configure<EndpointsOptions>(
    builder.Configuration.GetSection(EndpointsOptions.Name));

builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 40;
        options.Window = TimeSpan.FromSeconds(12);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 20;
    }));

builder.Services.AddHangfire(config =>
    config.UseMemoryStorage());

builder.Services.AddHangfireServer();

builder.Services.Scan(s => s.FromCallingAssembly()
    .AddClasses(c => c.AssignableTo<IService>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.AddScoped<IValidator<BatchIpRequest>, BatchIpRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRateLimiter();

app.UseHangfireDashboard();

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