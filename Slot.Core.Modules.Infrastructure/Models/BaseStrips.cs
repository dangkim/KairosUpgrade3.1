namespace Slot.Core.Modules.Infrastructure.Models
{
    using System.Collections.Generic;

    public abstract class BaseStrips : List<IReadOnlyList<int>>
    {
        protected BaseStrips(params IReadOnlyList<int>[] strips) : base(strips) {}
    }
}