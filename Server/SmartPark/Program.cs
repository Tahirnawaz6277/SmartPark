using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartPark.Configurations;
using SmartPark.Data.Contexts;
using SmartPark.Extensions;
using SmartPark.Middlwares;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev",
        policy => policy
            .WithOrigins("http://localhost:4200") // allow only specified origins
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Register services and jwt configurations to the container using the extension method
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);


//register db context calss

builder.Services.AddDbContext<ParkingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SmartParkConnectionString"),
        sqlOptions =>
        {
            sqlOptions.CommandTimeout(60);
            //sqlOptions.EnableRetryOnFailure();
        }));

// Register MediatR (scan whole assembly for handlers)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
 });


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureSwaggerAuthentication();

var app = builder.Build();

app.UseStaticFiles(); // Enables access to files in wwwroot
// Use CORS
app.UseCors("AllowAngularDev");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// Authentication must come before any middleware that checks authentication
app.UseAuthentication();
// Global exception handler should come after auth middleware but before controllers
app.UseMiddleware<GlobleExceptionHandlerMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
