using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var redisDbConnection = builder.Configuration.GetConnectionString("RedisDatabase") ??
     throw new InvalidOperationException("Connection string 'RedisDatabase'" + " not found.");

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisDbConnection));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "EvacuationPlanning API",
        Description = "An ASP.NET Core Web API",
        //TermsOfService = new Uri("https://example.com/terms"),
        //Contact = new OpenApiContact
        //{
        //    Name = "Prayote.Moomthong@gmail.com",
        //    //Url = new Uri("https://example.com/contact")
        //},
        //License = new OpenApiLicense
        //{
        //    Name = "Example License",
        //    Url = new Uri("https://example.com/license")
        //}
    }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });

}

app.MapGet("/", () => "Hello World!");

app.UseRouting();

app.MapControllers();

// CORS
//app.UseCors("some unique string");

app.Run();

