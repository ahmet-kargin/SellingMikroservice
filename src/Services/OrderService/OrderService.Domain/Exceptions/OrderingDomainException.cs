using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Exception;

public class OrderingDomainException : IOException
{
    public OrderingDomainException()
    { }

    public OrderingDomainException(string message)
        : base(message)
    { }

    public OrderingDomainException(string message, IOException innerException)
        : base(message, innerException)
    { }
}