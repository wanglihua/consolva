using System.Reflection;
using shunda.Core.Service;
using Microsoft.Extensions.DependencyInjection;

namespace shunda.Core;

public static class ServiceExtensions
{
    public static void AddCoreServices(this IServiceCollection services)
    {
        Assembly assembly = typeof(ServiceExtensions).Assembly;
        ServiceUtil.AddAssemblyService(services, assembly);
    }
}