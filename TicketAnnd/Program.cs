using System.Text.Json.Serialization;
using Serilog;
using System.Security.Claims;
using TicketAnnd;
using TicketAnnd.Extensions;
using Hangfire;
using Hangfire.Common;
using TicketAnnd.Infrastructure.Services;
using TicketAnnd.OutputCaching;

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
        o.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
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
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>()
            ?? (builder.Configuration["Cors:AllowedOrigins"]?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            ?? new[] { "http://localhost:3000", "http://localhost:3001", "http://localhost:5173" };

        policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("CompCache", new CompanyCachePolicy());
    options.AddPolicy("UserCache", new UserCachePolicy());
});

var app = builder.Build();

await app.SeedSuperAdminAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Schedule Hangfire recurring job to process outbox messages every minute using the service-based API
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate(
        "outbox-processor",
        Job.FromExpression<OutboxProcessor>(x => x.ProcessPendingAsync()),
        Cron.Minutely(),
        new RecurringJobOptions());
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();
