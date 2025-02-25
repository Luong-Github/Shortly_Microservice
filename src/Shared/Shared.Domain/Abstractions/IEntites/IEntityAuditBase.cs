using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Abstractions.IEntites
{
    public interface IEntityAuditBase<TKey> : IEntityBase<TKey>, IAuditBase, ISortDelete
    {
    }
}
