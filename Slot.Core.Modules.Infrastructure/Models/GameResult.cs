using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Core.Modules.Infrastructure.Models
{
    public class GameResult<T>
    {
        public T Payload { get; set; }
    }
}
