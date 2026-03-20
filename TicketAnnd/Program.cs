using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Serilog;
using System.Security.Claims;
using TicketAnnd;
using TicketAnnd.Extensions;
using TicketAnnd.Domain.Options;
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

builder.Services.AddCors();

// Configure CORS policy using CorsOptions via IConfigureOptions
builder.Services.AddSingleton<IConfigureOptions<Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions>>(sp =>
{
    var corsOptions = sp.GetRequiredService<IOptions<TicketAnnd.Domain.Options.CorsOptions>>().Value;
    return new ConfigureOptions<Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions>(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(corsOptions.AllowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
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
