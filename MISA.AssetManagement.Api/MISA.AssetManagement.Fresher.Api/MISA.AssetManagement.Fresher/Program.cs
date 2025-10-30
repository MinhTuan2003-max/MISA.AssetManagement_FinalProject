using Microsoft.AspNetCore.Mvc.ApplicationModels;
using MISA.AssetManagement.Fresher.Conventions;
using MISA.AssetManagement.Fresher.Middleware;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Service;
using MISA.Core.Services;
using MISA.Infrastructure.Reposiories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình route
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new LowercaseControllerTransformer()));
});

// Cấu hình Dapper - Tự động map snake_case sang camelCase
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MISA Fixed Asset Management API",
        Version = "v1",
        Description = "API quản lý tài sản cố định"
    });
});


// Đăng ký Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=3306;Database=misa_fixed_asset_management;User Id=root;Password=123456;";

// Đăng ký Repositories
builder.Services.AddScoped<IDepartmentRepository>(provider =>
    new DepartmentRepository(connectionString));

builder.Services.AddScoped<IFixedAssetCategoryRepository>(provider =>
    new FixedAssetCategoryRepository(connectionString));

builder.Services.AddScoped<IFixedAssetRepository>(provider =>
    new FixedAssetRepository(connectionString));

// Đăng ký Services
builder.Services.AddScoped<IFixedAssetService, FixedAssetService>();
builder.Services.AddScoped<IFixedAssetService, FixedAssetService>();
builder.Services.AddScoped<IBaseService<Department>, DepartmentService>();
builder.Services.AddScoped<IBaseService<FixedAssetCategory>, FixedAssetCategoryService>();


// Thêm CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware xử lý exception toàn cục
app.UseMiddleware<ExceptionMiddleware>();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MISA Fixed Asset API V1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
