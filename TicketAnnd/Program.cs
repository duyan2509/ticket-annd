using Serilog;
using TicketAnnd;
using TicketAnnd.Extensions;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy =
        System.Text.Json.JsonNamingPolicy.CamelCase;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration["Cors:AllowedOrigins"] ?? "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await app.SeedSuperAdminAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();
