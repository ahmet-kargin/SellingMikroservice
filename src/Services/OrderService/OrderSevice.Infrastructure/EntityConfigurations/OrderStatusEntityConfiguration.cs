﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.AggregateModel.OrderAggregate;
using OrderSevice.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSevice.Infrastructure.EntityConfigurations;

internal class OrderStatusEntityConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.ToTable("orderstatus", OrderDbContext.DEFAULT_SCHEMA);

        builder.HasKey(o => o.Id);
        builder.Property(i => i.Id).ValueGeneratedOnAdd();

        builder.Property(o => o.Id)
            .HasDefaultValue(1)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(o => o.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}
