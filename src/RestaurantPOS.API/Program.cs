using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestaurantPOS.API.Hubs;
using RestaurantPOS.API.Middleware;
using RestaurantPOS.Application;
using RestaurantPOS.Infrastructure;
using RestaurantPOS.Infrastructure.Data;
using RestaurantPOS.Infrastructure.Data.Seed;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/restaurantpos-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Application & Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers + FluentValidation
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<RestaurantPOS.Application.Validators.LoginValidator>();

// SignalR
builder.Services.AddSignalR();

// CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000", "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is not configured.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        // Allow SignalR to pass token via query string
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    context.Token = accessToken;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NovaliX API",
        Version = "v1",
        Description = "API para sistema de Punto de Venta NovaliX",
        Contact = new OpenApiContact { Name = "NovaliX", Email = "admin@restaurante.com" }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Ejemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await DataSeeder.SeedAsync(context);
        Log.Information("Database seeded successfully.");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Error seeding database.");
    }
}

// Middleware pipeline
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant POS API v1");
    c.RoutePrefix = "swagger";
});
// En desarrollo redirige a Swagger, en producción sirve el frontend
if (!app.Environment.IsDevelopment())
    app.MapGet("/", () => Results.Redirect("/index.html")).AllowAnonymous();
else
    app.MapGet("/", () => Results.Redirect("/swagger/index.html")).AllowAnonymous();

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<OrderHub>("/hubs/orders");

// Servir el frontend Angular desde wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.Run();
