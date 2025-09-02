using Microsoft.EntityFrameworkCore;
using SmartPark.Data.Contexts;
using SmartPark.Extensions;

var builder = WebApplication.CreateBuilder(args);


// Register services to the container using the extension method
builder.Services.AddApplicationServices();


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
