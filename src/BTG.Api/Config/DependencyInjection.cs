using BTG.Application.Interfaces;
using BTG.Application.Services;
using BTG.Infrastructure.Notifications;
using BTG.Infrastructure.Persistence;
using BTG.Infrastructure.Persistence.Repositories;
using BTG.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BTG.Api.Config;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration config)
    {
        // MongoOptions desde appsettings.json o variables de entorno
        services.Configure<MongoOptions>(config.GetSection(MongoOptions.SectionName));

        // Contexto Mongo
        services.AddSingleton<MongoContext>();

        // Repositorios
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IFondoRepository, FondoRepository>();
        services.AddScoped<ITransaccionRepository, TransaccionRepository>();

        // Servicios de dominio
        services.AddScoped<IFondoService, FondoService>();

        // Notificaciones (mock con logs en consola)
        services.AddScoped<INotificacionService, ConsoleNotificacionService>();

        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        services.AddScoped<IFondoService, FondoService>();

        return services;
    }

    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration cfg)
    {
        var key = cfg.GetSection("Jwt:Key").Get<string>()
                  ?? throw new InvalidOperationException("Jwt:Key missing");

        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Políticas de roles
        services.AddAuthorization(options =>
        {
            options.AddPolicy("OnlyAdmins", p => p.RequireRole("admin"));
            options.AddPolicy("OnlyClients", p => p.RequireRole("cliente"));
            options.AddPolicy("CanCancelFondo", p => p.RequireRole("admin", "cliente"));
        });

        Console.WriteLine($"Issuer esperado: {cfg["Jwt:Issuer"]}");
        Console.WriteLine($"Audience esperado: {cfg["Jwt:Audience"]}");
        Console.WriteLine($"Key esperado: {cfg["Jwt:Key"]}");

        // Configuración de autenticación JWT
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = cfg["Jwt:Issuer"],       
                ValidAudience = cfg["Jwt:Audience"],  
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ClockSkew = TimeSpan.Zero 
            };

            o.Events = new JwtBearerEvents
            {
                // Cuando falla autenticación
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");

                    if (context.Exception is SecurityTokenInvalidSignatureException)
                        Console.WriteLine(" Firma inválida");
                    if (context.Exception is SecurityTokenExpiredException)
                        Console.WriteLine(" Token expirado");
                    if (context.Exception is SecurityTokenInvalidIssuerException)
                        Console.WriteLine(" Issuer inválido");
                    if (context.Exception is SecurityTokenInvalidAudienceException)
                        Console.WriteLine(" Audience inválida");

                    return Task.CompletedTask;
                },

                // Cuando el token no está presente o no cumple
                OnChallenge = context =>
                {
                    Console.WriteLine($" JWT Challenge: {context.Error}, {context.ErrorDescription}");
                    return Task.CompletedTask;
                },

                // Log para saber si llega el token
                OnMessageReceived = context =>
                {
                    var token = context.Request.Headers["Authorization"].ToString();
                    if (string.IsNullOrWhiteSpace(token))
                        Console.WriteLine(" No se encontró token en el header");
                    else
                        Console.WriteLine($" Token recibido en header: {token}");
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}