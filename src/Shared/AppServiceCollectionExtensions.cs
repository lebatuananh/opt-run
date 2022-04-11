using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shared.DependencyInjection;
using Shared.Extensions;

namespace Shared
{
    public static class AppServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoScanInjection(this IServiceCollection services)
        {
            RuntimeHelper.GetAllCoreAssemblies().ToList().ForEach(a =>
            {
                a.GetTypes().Where(t => typeof(IPrivateDependency).IsAssignableFrom(t) && t.IsClass).ToList().ForEach(
                    t =>
                    {
                        var serviceType = t.GetInterface($"I{t.Name}");
                        if ((serviceType ?? t).GetInterface(nameof(ISingletonDependency)) != null)
                        {
                            if (serviceType != null)
                            {
                                services.AddSingleton(serviceType, t);
                            }
                            else
                            {
                                services.AddSingleton(t);
                            }
                        }
                        else if ((serviceType ?? t).GetInterface(nameof(IScopedDependency)) != null)
                        {
                            if (serviceType != null)
                            {
                                services.AddScoped(serviceType, t);
                            }
                            else
                            {
                                services.AddScoped(t);
                            }
                        }
                        else if ((serviceType ?? t).GetInterface(nameof(ITransientDependency)) != null)
                        {
                            if (serviceType != null)
                            {
                                services.AddTransient(serviceType, t);
                            }
                            else
                            {
                                services.AddTransient(t);
                            }
                        }
                        else
                        {
                            if (serviceType != null)
                            {
                                services.AddTransient(serviceType, t);
                            }
                            else
                            {
                                services.AddTransient(t);
                            }
                        }
                    });
            });
            return services;
        }
    }
}