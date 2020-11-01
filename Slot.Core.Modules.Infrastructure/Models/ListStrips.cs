namespace Slot.Core.Modules.Infrastructure.Models
{
    using System.Collections.Generic;

    public abstract class ListStrips : List<BaseStrips>
    {
        protected ListStrips(params BaseStrips[] strips) : base(strips)
        {
        }
    }
}