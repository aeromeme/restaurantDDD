using Application.UseCase.CategoryCase;
using Application.UseCase.OrderCase;
using Application.UseCase.ProductCase;
using Domain.Entities;
using Domain.Ports;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Filters;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

//add persistence
builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository> ();


//unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//automapper
//builder.Services.AddAutoMapper(typeof(Program).Assembly);
//builder.Services.AddAutoMapper(typeof(Repository.Mappers.EntityModelMappingProfile).Assembly);
//builder.Services.AddAutoMapper(typeof(Application.Mappers.EntityModelMappingProfile).Assembly);

//use cases
builder.Services.AddScoped<GetAllCategories>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<OrderService>();


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

app.UseAuthorization();

app.MapControllers();

app.Run();
