using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using shunda.Core.Service;

namespace shunda.Service;

public static class ServiceExtensions
{
    public static void AddAppServices(this IServiceCollection services)
    {
        Assembly assembly = typeof(ServiceExtensions).Assembly;
        ServiceUtil.AddAssemblyService(services, assembly);
    }

    public static void AddAppServicesForceSingleton(this IServiceCollection services)
    {
        Assembly assembly = typeof(ServiceExtensions).Assembly;
        ServiceUtil.AddAssemblyServiceForceSingleton(services, assembly);
    }    
}
