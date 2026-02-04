using EFCore.NamingConventions;
using FileService.Data;
using FileService.Options;
using FileService.Services;
using FileService.Services.Abstractions;
using FileService.Services.Clients;
using FileService.Services.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.Configure<MovieServiceOptions>(builder.Configuration.GetSection("MovieService"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FileDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    options
        .UseNpgsql(connectionString, npgsql =>
        {
            npgsql.MigrationsHistoryTable("__FileServiceMigrations");
        })
        .UseSnakeCaseNamingConvention();
});

builder.Services.AddHttpClient<IMovieCatalogClient, MovieCatalogClient>((sp, client) =>
{
    var movieOptions = sp.GetRequiredService<IOptions<MovieServiceOptions>>().Value;
    client.BaseAddress = new Uri(movieOptions.BaseUrl);
});

builder.Services.AddScoped<IFileStorageService, FileSystemStorageService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

var app = builder.Build();

await EnsureDatabaseAsync(app.Services, app.Logger);

if (app.Environment.IsDevelopment())
{ 
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

static async Task EnsureDatabaseAsync(IServiceProvider services, ILogger logger)
{
    using var scope = services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FileDbContext>();
    await dbContext.Database.MigrateAsync();

    logger.LogInformation("File service database ready: {Database}", dbContext.Database.GetDbConnection().Database);
}
