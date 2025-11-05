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

    try
    {
        // Cấu hình Redis với retry mechanism
        var configOptions = ConfigurationOptions.Parse(redisConnectionString);
        configOptions.AbortOnConnectFail = false; // Không crash nếu Redis chưa sẵn sàng
        configOptions.ConnectRetry = 5;           // Retry 5 lần
        configOptions.ConnectTimeout = 10000;     // Timeout 10 giây
        configOptions.SyncTimeout = 5000;         // Sync timeout 5 giây
        configOptions.AsyncTimeout = 5000;        // Async timeout 5 giây
        
        var redis = ConnectionMultiplexer.Connect(configOptions);
        
        // Log khi kết nối thành công
        Console.WriteLine($"Connected to Redis: {redis.GetEndPoints().FirstOrDefault()}");
        
        builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
        builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
    }
    catch (Exception ex)
    {
        // Log lỗi nhưng không crash app
        Console.WriteLine($" Redis connection failed: {ex.Message}");
        Console.WriteLine(" Application will continue without Redis cache");
        
        // Fallback: Đăng ký một service cache giả (nếu cần)
        // Hoặc có thể comment dòng này nếu Redis là bắt buộc
        builder.Services.AddSingleton<IRedisCacheService>(
            provider => throw new InvalidOperationException("Redis is not available")
        );
    }

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

    // Swagger - Luôn bật (cả Development và Production)
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MISA Fixed Asset API V1");
        c.RoutePrefix = string.Empty; // Swagger UI tại root URL
    });

    // Không dùng HTTPS redirect trong Docker (gây lỗi)
    // app.UseHttpsRedirection();

    app.UseCors("AllowAll");
    app.UseAuthorization();
    app.MapControllers();
    
    // Health check endpoint
    app.MapGet("/health", () => Results.Ok(new 
    { 
        status = "healthy", 
        timestamp = DateTime.UtcNow 
    }));
}