using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Modules.Common.API.Abstractions;
using Modules.Common.Application.Extensions;
using Modules.Users.Features.Middlewares;
using Modules.Users.Features.InternalApi;
using Modules.Users.PublicApi;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class UsersModuleRegistration
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddUsersModuleApi()
            .AddUsersInfrastructure(configuration);
    }
    
    private static IServiceCollection AddUsersModuleApi(this IServiceCollection services)
    {
        services.AddScoped<UsersModuleApi>();
        services.AddScoped<IUsersModuleApi>(provider => provider.GetRequiredService<UsersModuleApi>());

        services.RegisterApiEndpointsFromAssemblyContaining(typeof(UsersModuleRegistration));
        
        services.RegisterHandlersFromAssemblyContaining(typeof(UsersModuleRegistration));
        
        services.AddValidatorsFromAssembly(typeof(UsersModuleRegistration).Assembly);

        return services;
    }
}

public class StocksMiddlewareConfigurator : IModuleMiddlewareConfigurator
{
    public IApplicationBuilder Configure(IApplicationBuilder app)
    {
        return app.UseMiddleware<CheckRevocatedTokensMiddleware>();
    }
}
