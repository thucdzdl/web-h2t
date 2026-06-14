using Microsoft.EntityFrameworkCore;
using MusicApp.Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// ✅ Chỉ AddCors một lần
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ✅ Chỉ AddControllers một lần
builder.Services.AddControllers();

builder.Services.AddDbContext<MusicAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi();

var app = builder.Build();

// mở khóa cors
app.UseCors("AllowAll");
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
