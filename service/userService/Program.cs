using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UserService.Data;
using UserService.Options;
using UserService.Services;
using UserService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<MovieServiceOptions>(builder.Configuration.GetSection("MovieService"));

builder.Services.AddDbContext<UserDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Connection string 'Default' is not configured.");
    }

    options.UseNpgsql(connectionString);
    options.UseSnakeCaseNamingConvention();
});

builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IUserDataService, UserDataService>();
builder.Services
    .AddHttpClient<IMovieMetadataClient, MovieMetadataClient>()
    .ConfigureHttpClient((sp, client) =>
    {
        var options = sp.GetRequiredService<IOptions<MovieServiceOptions>>().Value;
        client.BaseAddress = new Uri(options.BaseUrl ?? "http://movie-service:8080");
        client.Timeout = TimeSpan.FromSeconds(15);
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var signingKey = jwtSettings.GetValue<string>("Key") ?? throw new InvalidOperationException("Jwt:Key is not configured.");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
        ValidAudience = jwtSettings.GetValue<string>("Audience"),
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MovieHub User Service",
        Version = "v1",
        Description = "Endpoints backing /api/me* routes"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    await db.Database.MigrateAsync();
}

await app.RunAsync();
