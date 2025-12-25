using UserService.API.Extensions;
using UserService.Application;
using UserService.Infrastructure;

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

app.UseHttpsRedirection();

// Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
