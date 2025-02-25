using Shared.Domain.Abstractions.IEntites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Abstractions
{
    public class EntityBase<TKey>  : IEntityBase<TKey>
    {
        [Key]
        public TKey Id { get; init; }
    }
}
