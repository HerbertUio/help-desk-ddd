using Application.UseCases.UserUseCases.UserAuthentication;
using Application.UseCases.UserUseCases.UserManagement;
using Domain.Factories;
using Domain.IRepositories;
using Infrastructure.Database.EntityFramework.Context;
using Infrastructure.Database.EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Ioc.Di;

public static class HelpDeskDependencyInjection
{
    public static IServiceCollection AddDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddDomainFactories();
        services.AddApplicationUseCases();
        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? configuration["ConnectionStrings:remoteConnection"]
                               ?? throw new ArgumentNullException(nameof(configuration), "Connection string ('DefaultConnection' o 'remoteConnection') no encontrada.");

        services.AddDbContext<HelpDeskDbContext>(options =>
            options.UseNpgsql(connectionString));
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        // Registrar otros repositorios aquí...
        return services;
    }

    private static IServiceCollection AddDomainFactories(this IServiceCollection services)
    {
        services.AddScoped<UserFactory>();
        // Registrar otras factories aquí...
        return services;
    }

    private static IServiceCollection AddApplicationUseCases(this IServiceCollection services)
    {
        services.AddScoped<LoginUseCase>();
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<GetAllUsersUseCase>();
        services.AddScoped<GetUserByIdUseCase>();
        services.AddScoped<UpdateUserUseCase>();
        services.AddScoped<DeleteUserUseCase>();
        return services;
    }
}