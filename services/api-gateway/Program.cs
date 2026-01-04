using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add Health Checks
builder.Services.AddHealthChecks();

// Add Swagger for Gateway-specific endpoints (health, etc.)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MovieHub API Gateway",
        Version = "v1",
        Description = "API Gateway for MovieHub microservices using YARP"
    });
});

// Add CORS if needed
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");
    });
}

app.UseCors();

// Map YARP reverse proxy FIRST - this handles all API routing
app.MapReverseProxy();

// Map health checks
app.MapHealthChecks("/health");

// Add gateway-specific endpoints without MapControllers to avoid conflicts
app.MapGet("/api/gateway/health", () => Results.Ok(new 
{ 
    status = "healthy", 
    service = "api-gateway", 
    timestamp = DateTime.UtcNow 
}));

app.Run();
