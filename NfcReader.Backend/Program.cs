using Microsoft.EntityFrameworkCore;
using NfcReader.Backend.Contexts;
using NfcReader.Backend.Services;
using NfcReader.Backend.Services.Interfaces;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(x =>
{
    x.EnableSensitiveDataLogging();
    x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), x => x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
});

builder.Services.AddTransient<IClockingService, ClockingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.MapOpenApi();
app.MapScalarApiReference();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
