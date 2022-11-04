using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace shunda.Core.Service;

#nullable disable

public class ServiceUtil
{
    public static void AddAssemblyService(IServiceCollection services, Assembly assembly)
    {
        IList<Type> typeList = assembly.GetTypes().Where(t => t.IsClass && (!t.IsAbstract)).ToList();
        foreach (Type type in typeList)
        {
            ServiceAttribute serviceAttribute = type.GetCustomAttribute<ServiceAttribute>();
            if (serviceAttribute != null)
            {
                if (serviceAttribute.ServiceType == ServiceType.Singleton)
                {
                    services.AddSingleton(type);
                }
                else if (serviceAttribute.ServiceType == ServiceType.Scoped)
                {
                    services.AddScoped(type);
                }
                else if (serviceAttribute.ServiceType == ServiceType.Transient)
                {
                    services.AddTransient(type);
                }
            }
        }
    }

    /// <summary>
    /// 强制添加 service 为 Singleton
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    public static void AddAssemblyServiceForceSingleton(IServiceCollection services, Assembly assembly)
    {
        IList<Type> typeList = assembly.GetTypes().Where(t => t.IsClass && (!t.IsAbstract)).ToList();
        foreach (Type type in typeList)
        {
            ServiceAttribute serviceAttribute = type.GetCustomAttribute<ServiceAttribute>();
            if (serviceAttribute != null)
            {
                services.AddSingleton(type);
            }
        }
    }
}
