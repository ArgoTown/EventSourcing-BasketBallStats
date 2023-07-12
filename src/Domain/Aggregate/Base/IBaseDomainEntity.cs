using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballStats.Domain.Aggregate.Base;

public interface IBaseDomainEntity
{
    Guid Id { get; }
}
