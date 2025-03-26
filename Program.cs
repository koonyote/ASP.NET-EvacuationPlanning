using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using EvacuationPlanning.Zones;
using EvacuationPlanning.Vehicles;
using EvacuationPlanning.Processor;

[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var redisDbConnection = builder.Configuration.GetConnectionString("RedisAzure") ??
     throw new InvalidOperationException("Connection string 'RedisDatabase'" + " not found.");

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisDbConnection));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Add services to the container
builder.Services.AddScoped<ZoneService>();
builder.Services.AddScoped<VehicleService>();
builder.Services.AddScoped<ProcessorService>();

// Interface implement & Dependency Injection 
builder.Services.AddScoped<IZoneController, ZoneController>();
builder.Services.AddScoped<IVehicleController, VehicleController>();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "EvacuationPlanning API",
        Description = "An ASP.NET Core Web API",
    }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    //app.UseHttpsRedirection();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

    //c.RoutePrefix = string.Empty;
});

app.MapGet("/", () => "Hello World!");

app.UseRouting();

app.MapControllers();

// CORS
//app.UseCors("some unique string");

app.Run();

