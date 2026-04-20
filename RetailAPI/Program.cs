using RetailAPI.Config;
using RetailAPI.Extensions;
using RetailAPI.Logs;
using RetailAPI.Middleware;
using RetailAPI.Repositories;
using RetailAPI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// Configure Serilog
LoggerService.ConfigureSerilog();
builder.Host.UseSerilog();

// Register Services
builder.Services.AddControllers();
builder.Services.AddDbContextService(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.AddSwaggerService();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddModule2Services();

// Register IHttpClientFactory for EmailService
builder.Services.AddHttpClient();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<CartRepository>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderRepository>();  // ← add if needed
builder.Services.AddScoped<OrderService>();
// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
// Bind Config Sections
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

var app = builder.Build();

// Middleware Pipeline
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Retail API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();

// Ensure Uploads folder exists
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
if (!Directory.Exists(uploadsPath))
    Directory.CreateDirectory(uploadsPath);

app.UseStaticFiles();
app.MapControllers();

Log.Information("Retail API started successfully.");
app.Run();
