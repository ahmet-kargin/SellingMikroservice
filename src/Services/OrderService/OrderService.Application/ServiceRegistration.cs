using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationRegistration(this IServiceCollection services, Type startup)
    {
        var assm = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assm));
        services.AddAutoMapper(assm);

        return services;
    }
}
