using EFCore.NamingConventions;
using Microsoft.EntityFrameworkCore;
using MovieService.Data;
using MovieService.Services;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MovieDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
           .UseSnakeCaseNamingConvention();
});

builder.Services.AddScoped<IMovieCatalogService, MovieCatalogService>();

var app = builder.Build();

await ApplyMigrationsAndSeedAsync(app.Services, app.Logger);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

static async Task ApplyMigrationsAndSeedAsync(IServiceProvider services, ILogger logger)
{
    using var scope = services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
    await dbContext.Database.EnsureCreatedAsync();

    var seedLogger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("MovieSeeder");

    await SeedData.EnsureSeedDataAsync(dbContext, seedLogger);
    logger.LogInformation("Movie service started with database {Database}", dbContext.Database.GetDbConnection().Database);
}
