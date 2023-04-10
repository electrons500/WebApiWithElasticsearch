using Elasticsearch.Net;
using Microsoft.OpenApi.Models;
using Nest;
using WebApiWithElasticsearch.Data.Service;
using static System.Reflection.Metadata.BlobBuilder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Elasticsearch", Version = "v1" });
});

//Elasticsearch
var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
var settings = new ConnectionSettings(pool);
var client = new ElasticClient(settings);
builder.Services.AddSingleton(client);

builder.Services.AddSingleton<TweetService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
