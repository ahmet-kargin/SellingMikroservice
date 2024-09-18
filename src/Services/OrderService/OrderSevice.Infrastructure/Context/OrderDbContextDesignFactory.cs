using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace OrderSevice.Infrastructure.Context;

public class OrderDbContextDesignFactory : IDesignTimeDbContextFactory<OrderDbContext>
{
    public OrderDbContextDesignFactory()
    {
    }

    public OrderDbContext CreateDbContext(string[] args)
    {
        var connStr = "Data Source=c_sqlserver;Initial Catalog=order;Persist Security Info=True;User ID=sa;Password=Salih123!";

        var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>()
            .UseSqlServer(connStr);

        return new OrderDbContext(optionsBuilder.Options, new NoMediator());
    }

    public class NoMediator : IMediator, ISender // ISender ekleniyor
    {
        // ISender.Send<TRequest> metodu
        public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest<TResponse>
        {
            return Task.FromResult<TResponse>(default);
        }

        // IMediator metotları
        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return default;
        }

        public IAsyncEnumerable<object> CreateStream(object request, CancellationToken cancellationToken = default)
        {
            return default;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            return Task.CompletedTask;
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<TResponse>(default);
        }

        public Task<object> Send(object request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<object>(default);
        }

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
        {
            throw new NotImplementedException();
        }
    }

}
