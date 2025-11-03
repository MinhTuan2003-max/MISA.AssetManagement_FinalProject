using Microsoft.AspNetCore.Mvc.ApplicationModels;
using MISA.AssetManagement.Fresher.Conventions;
using MISA.AssetManagement.Fresher.Middleware;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Service;
using MISA.Core.Services;
using MISA.Infrastructure.Reposiories;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Configure Services
ConfigureServices(builder);

var app = builder.Build();

// Configure Middleware
ConfigureMiddleware(app);

app.Run();

/// <summary>
/// Cấu hình toàn bộ DI (Dependency Injection) cho ứng dụng.
/// CreatedBy: HMTuan (03/11/2025)
/// </summary>
/// <param name="builder">WebApplicationBuilder</param>
void ConfigureServices(WebApplicationBuilder builder)
{
    // Cấu hình Controllers và Route
    builder.Services.AddControllers(options =>
    {
        // Convention để route controller tự động lowercase
        options.Conventions.Add(
            new RouteTokenTransformerConvention(new LowercaseControllerTransformer())
        );
    });

    // Cấu hình Swagger
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

    // Cấu hình Dapper - Cho phép tự động map snake_case sang PascalCase
    Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

    // Connection String
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection not found in configuration");

    var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
        ?? throw new InvalidOperationException("Redis connection string not found in configuration");

    var redis = ConnectionMultiplexer.Connect(redisConnectionString);
    builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
    builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();


    // Đăng ký Repository (Data Access Layer)
    builder.Services.AddScoped<IDepartmentRepository>(
        provider => new DepartmentRepository(connectionString)
    );

    builder.Services.AddScoped<IFixedAssetCategoryRepository>(
        provider => new FixedAssetCategoryRepository(connectionString)
    );

    builder.Services.AddScoped<IFixedAssetRepository>(
        provider => new FixedAssetRepository(connectionString)
    );

    // Đăng ký Service (Business Layer)
    builder.Services.AddScoped<IFixedAssetService, FixedAssetService>();
    builder.Services.AddScoped<IBaseService<Department>, DepartmentService>();
    builder.Services.AddScoped<IBaseService<FixedAssetCategory>, FixedAssetCategoryService>();

    // Cấu hình CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
}

/// <summary>
/// Cấu hình pipeline middleware cho ứng dụng.
/// CreatedBy: HMTuan (03/11/2025)
/// </summary>
/// <param name="app">WebApplication</param>
void ConfigureMiddleware(WebApplication app)
{
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
}
