using System.IdentityModel.Tokens.Jwt;
using UserService.API.Extensions;
using UserService.Application;
using UserService.Infrastructure;

// Clearing the default claim type map to prevent claim renaming (e.g. "sub" to "nameidentifier")
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure JWT Authentication (ONLY validation, NOT creation)
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configure Swagger with JWT support
builder.Services.AddSwaggerWithJwt();

// Add Application layer services
builder.Services.AddApplication();

// Add Infrastructure layer (EF Core + PostgreSQL)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "User Service API v1");
    });
}

// Disable HTTPS redirection so the service works behind HTTP-only reverse proxies (Docker gateway)
// The gateway terminates traffic on HTTP, so forcing HTTPS here produces 307 redirects to container hostnames
// which the browser cannot resolve.

// Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
