using EPS.API;
using EPS.API.ActionFilters;
using EPS.Application;
using EPS.Infrastructure.BackgroundJobs;
using EPS.Persistence;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
builder.Services.AddScoped(typeof(ValidationModelFilterAttribute<>));
builder.Services.AddScoped<ValidationFilterAttribute>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddApi();
builder.Services.AddExceptionHandler<GlobalExceptionalHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseHangfireDashboard();
app.MapControllers();

app.Run();