using System;

namespace Slot.Core.Services.Exceptions
{
    [Serializable]
    public class WalletException : Exception
    {
        public WalletException() { }

        public WalletException(string message)
            : base(message) { }

        public WalletException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
