using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartPark.DbContext;
using SmartPark.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//register db context calss
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("SmartParkConnectionString"));
});

builder.Services.AddIdentity<Users, IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<Users>>("SmartPark")
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
