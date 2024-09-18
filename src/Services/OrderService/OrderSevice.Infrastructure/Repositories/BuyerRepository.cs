using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModel.BuyerAggreagate;
using OrderSevice.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSevice.Infrastructure.Repositories;

public class BuyerRepository : GenericRepository<Buyer>, IBuyerRepository
{
    public BuyerRepository(OrderDbContext dbContext) : base(dbContext)
    {
    }
}