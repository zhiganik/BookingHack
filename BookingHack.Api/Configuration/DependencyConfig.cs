using System.Text;
using BookingHack.Application;
using BookingHack.Application.Services;
using BookingHack.Application.Services.Abstractions;
using BookingHack.Core.Swagger;
using BookingHack.Domain.Constants;
using BookingHack.Domain.Enums;
using BookingHack.Domain.Models;
using BookingHack.Infrastructure.PostgreSql;
using BookingHack.Infrastructure.PostgreSql.DbContext;
using BookingHack.Infrastructure.Redis;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Exceptions;

namespace BookingHack.Core.Configuration;

public static class DependencyConfig
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        return services
            .AddSerilogLogging(configuration)
            .AddIdentity()
            .AddAuthentication(configuration)
            .AddApplication()
            .AddInfrastructure(configuration)
            .AddRedisInfrastructure(configuration)
            .AddOpenApiSpec();
    }

    private static IServiceCollection AddOpenApiSpec(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "BookingHack API",
                Version = "v1",
                Description = "The BookingHack API",
                Contact = new OpenApiContact
                {
                    Name = "Mykyta",
                    Email = "example@gmail.com",
                }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token.",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer",
                }
            );
            
            options.AddSecurityRequirement(document => new()
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });

            options.OperationFilter<AllowAnonymousOperationFilter>();
            options.OperationFilter<AuthorizationDescriptionOperationFilter>();
        });

        return services;
    }
    
    private static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((services, lc) => lc
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("ServiceName", "SomeService")
            .WriteTo.Console());
        
        return services;
    }
    
    private static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ICompanyMemberService, CompanyMemberService>();
        services.AddScoped<RefreshTokenService>();

        return services;
    }
    
    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<BookingHackDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
    
    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<JwtService>();
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,           
                ValidateAudience = true,         
                ValidateLifetime = true,        
                ValidateIssuerSigningKey = true, 

                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured")))
            };
        });

        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.CompanyManager, policy => policy.RequireClaim(Claims.CompanyRole, Enum.GetNames(typeof(CompanyRole))))
            .AddPolicy(Policies.CompanyOwner, policy => policy.RequireClaim(Claims.CompanyRole, nameof(CompanyRole.Owner)));
        
        return services;
    }
}