using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces.Repositories;
using OrderSevice.Infrastructure.Context;
using OrderSevice.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSevice.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddPersistenceRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(opt =>
        {
            opt.UseSqlServer(configuration["OrderDbConnectionString"]);
            opt.EnableSensitiveDataLogging();
        });

        services.AddScoped<IBuyerRepository, BuyerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>()
            .UseSqlServer(configuration["OrderDbConnectionString"]);

        using var dbContext = new OrderDbContext(optionsBuilder.Options, null);
        dbContext.Database.EnsureCreated();
        dbContext.Database.Migrate();

        return services;
    }
}
}