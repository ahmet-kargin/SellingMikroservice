﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OrderService.Domain.AggregateModel.BuyerAggreagate;
using OrderService.Domain.AggregateModel.OrderAggregate;
using OrderService.Domain.SeedWork;
using OrderSevice.Infrastructure.EntityConfigurations;
using OrderSevice.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace OrderSevice.Infrastructure.Context;

public class OrderDbContext : DbContext, IUnitOfWork
{
    public const string DEFAULT_SCHEMA = "ordering";
    private readonly IMediator mediator;

    public OrderDbContext() : base()
    {

    }

    public OrderDbContext(DbContextOptions<OrderDbContext> options, IMediator mediator) : base(options)
    {
        this.mediator = mediator;
    }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<PaymentMethod> Payments { get; set; }

    public DbSet<Buyer> Buyers { get; set; }

    public DbSet<CardType> CardTypes { get; set; }

    public DbSet<OrderStatus> OrderStatus { get; set; }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await mediator.DispatchDomainEventsAsync(this);
        await base.SaveChangesAsync(cancellationToken);
        return true;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new BuyerEntityConfiguration());
        modelBuilder.ApplyConfiguration(new OrderStatusEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentMethodEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CardTypeEntityConfiguration());
    }
}
