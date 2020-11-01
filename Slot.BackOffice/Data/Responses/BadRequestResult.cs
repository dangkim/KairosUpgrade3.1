using Microsoft.AspNetCore.Mvc;
using Slot.Model;

namespace Slot.BackOffice.Data.Responses
{
    /// <summary>
    /// Standard response for bad requests.
    /// </summary>
    public class BadRequestResult : BadRequestObjectResult
    {
        /// <summary>
        /// Create a standard response for bad requests.
        /// </summary>
        /// <param name="value">Message to be returned.</param>
        /// <param name="errorCode">Error code of type <see cref="ErrorCode"/>.</param>
        public BadRequestResult(string value, ErrorCode errorCode) : base(errorCode)
        {
            Value = new
            {
                Value = value,
                Error = errorCode,
                IsError = true
            };
        }
    }
}
