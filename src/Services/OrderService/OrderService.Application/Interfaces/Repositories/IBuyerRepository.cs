﻿using OrderService.Domain.AggregateModel.BuyerAggreagate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Interfaces.Repositories;

public interface IBuyerRepository: IGenericRepository<Buyer>
{
}
