using System.Text;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using TicketAnnd.Application.Behaviors;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Options;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Infrastructure.Persistence;
using TicketAnnd.Infrastructure.Persistence.Repositories;
using TicketAnnd.Infrastructure.Services;
using Hangfire;
using Hangfire.PostgreSql;
using TicketAnnd.Infrastructure.Persistence.Mongo;

namespace TicketAnnd.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsConfiguration(configuration);
        services.AddPostgres(configuration);
        services.AddMongo();
        services.AddHangfireJobs(configuration);
        services.AddRedis();
        services.AddMapper();
        services.AddCqrs();
        services.AddAuth();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddRepositories();
        services.AddApplicationServices();
        // Register OutboxProcessor for Hangfire-invoked processing
        services.AddScoped<OutboxProcessor>();

        return services;
    }

    public static IServiceCollection AddOptionsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<MailtrapOptions>(configuration.GetSection(MailtrapOptions.SectionName));
        services.Configure<SeedAdminOptions>(configuration.GetSection(SeedAdminOptions.SectionName));
        services.Configure<FrontendOptions>(configuration.GetSection(FrontendOptions.SectionName));
        services.Configure<MongoOptions>(configuration.GetSection(MongoOptions.SectionName));
        services.Configure<Domain.Options.CorsOptions>(configuration.GetSection(Domain.Options.CorsOptions.SectionName));
        services.Configure<Domain.Options.RedisOptions>(configuration.GetSection(Domain.Options.RedisOptions.SectionName));

        return services;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        // Configure JwtBearerOptions using IOptions<JwtOptions> from DI
        services.AddSingleton<IConfigureOptions<JwtBearerOptions>>(sp =>
        {
            var jwtOptions = sp.GetRequiredService<IOptions<JwtOptions>>().Value;
            return new ConfigureNamedOptions<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtOptions.Key)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        });

        return services;
    }

    public static IServiceCollection AddHangfireJobs(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(connectionString));

        services.AddHangfireServer();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ISlaPolicyRepository, SlaPolicyRepository>();
        services.AddScoped<IUserCompanyRoleRepository, UserCompanyRoleRepository>();
        services.AddScoped<IInvitationRepository, InvitationRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<ITicketLogRepository, TicketLogRepository>();
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEmailSender, MailtrapEmailSender>();
        return services;
    }

    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<TicketAnndDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            var mongoOptions = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
            if (string.IsNullOrWhiteSpace(mongoOptions.ConnectionString))
                throw new InvalidOperationException("Mongo:ConnectionString is not configured.");
            return new MongoClient(mongoOptions.ConnectionString);
        });

        services.AddSingleton(sp =>
        {
            var mongoOptions = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoOptions.Database);
        });

        return services;
    }

    public static IServiceCollection AddRedis(this IServiceCollection services)
    {
        services.AddStackExchangeRedisCache(options => { });

        // Wire RedisOptions into RedisCacheOptions via IConfigureOptions
        services.AddSingleton<IConfigureOptions<
            Microsoft.Extensions.Caching.StackExchangeRedis.RedisCacheOptions>>(sp =>
        {
            var redisOptions = sp.GetRequiredService<IOptions<Domain.Options.RedisOptions>>().Value;
            return new ConfigureOptions<
                Microsoft.Extensions.Caching.StackExchangeRedis.RedisCacheOptions>(options =>
            {
                if (!string.IsNullOrWhiteSpace(redisOptions.Connection))
                {
                    options.Configuration = redisOptions.Connection;
                }
            });
        });

        return services;
    }

    public static IServiceCollection AddMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Program).Assembly);
        return services;
    }

    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
