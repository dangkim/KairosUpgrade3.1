using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Core.Modules.Infrastructure.Exceptions
{
    public class InvalidGameModuleImplementation : Exception
    {
        public InvalidGameModuleImplementation()
        {
        }

        public InvalidGameModuleImplementation(string module, string message) : base($"{module} : {message}")
        {
        }
    }
}
