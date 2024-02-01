using HappyFarm.Data.Models;
using HappyFarm.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HappyFarmContext>(options=>options.UseSqlite("Data Source=HappyFarm.db"));

using(var provider = builder.Services.BuildServiceProvider())
    provider.GetRequiredService<HappyFarmContext>().Database.EnsureCreated();


builder.Services.AddSingleton<DeviceServices>();
builder.Services.AddSingleton<WokerServices>();

var app = builder.Build();

var deviceServices = app.Services.GetService<DeviceServices>();
var WorkerSetvices = app.Services.GetService<WokerServices>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
