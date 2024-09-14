using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOcelot();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())  // Proje k�k dizinini temel dizin olarak belirler
    .AddJsonFile("Configurations/ocelot.json", optional: false, reloadOnChange: true)  // ocelot.json yap�land�rma dosyas�n� ekler
    .AddEnvironmentVariables();  // Ortam de�i�kenlerini ekler


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

app.UseOcelot();

app.Run();
