using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApi.Filters;
using WebApi.Models;
using WebApi.Models.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ActionLogger>();
    options.Filters.Add<MyExceptionFilter>();
});

// Add DbContext
builder.Configuration.AddUserSecrets("b71ee39b-f60d-4e7c-a95b-95cb0de7b55e");
string? conection = builder.Configuration["ConnectionStrings:cnn"];
builder.Services.AddDbContext<UserDatabaseDbContext>(options => options.UseSqlServer(conection));

// Add Identity
builder.Services.AddIdentity<User,IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<UserDatabaseDbContext>();

// Config JsonOptions
builder.Services.Configure<JsonOptions>(c => 
    c.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    );

// Add Cache Service
builder.Services.AddMemoryCache();

var app = builder.Build();

// LOG APLICATION
app.Lifetime.ApplicationStarted.Register(() => Log.Information("Application started"));
app.Lifetime.ApplicationStopped.Register(() => Log.Information("Application stopped"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseHttpsRedirection();

app.MapControllers();


app.Run();
