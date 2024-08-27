using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
namespace EventBus.RabbitMQ;

public class RabbitMQPersistentConnection : IDisposable
{
    private IConnection connection;
    private readonly int retryCount;
    private readonly IConnectionFactory connectionFactory;
    private object lock_object = new object();
    private bool _disposed;
    public RabbitMQPersistentConnection(IConnectionFactory connectionFactory,int retryCount =5)
    {
        this.connection = connectionFactory.CreateConnection();
        this.retryCount = retryCount;
    }

    public bool IsConnected => connection != null && connection.IsOpen;

    public IModel CreateModel()
    {
        return connection.CreateModel();
    }
    public void Dispose()
    {
        _disposed = true;
        connection?.Dispose();
    }

    public bool TryConnect()
    {
        lock (lock_object)
        {
            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    // Log exception or handle it accordingly
                });

            policy.Execute(() =>
            {
                connection = connectionFactory.CreateConnection();
            });

            if(IsConnected)
            {
                connection.ConnectionShutdown += Connection_ConnectionShutdown;
                connection.CallbackException += Connection_CallbackExeption;
                connection.ConnectionBlocked += Connection_ConnectionBlocked;


                return true;
            }
            return false;
        }

        return connection != null && connection.IsOpen;
    }

    private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {

        TryConnect();
    }

    private void Connection_CallbackExeption(object sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
    {
        TryConnect();
    }

    private void Connection_ConnectionBlocked(object sender, global::RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
    {
        TryConnect();
    }

}
