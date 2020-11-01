using Slot.Core.Data.Attributes.SqlBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.BackOffice.Data.Queries
{
    public abstract class BaseQuery
    {
        [Excluded]
        public virtual int? Limit { get; set; }

        [Excluded]
        public virtual string Ordering { get; set; }
    }
}
