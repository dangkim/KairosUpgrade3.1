using Microsoft.AspNetCore.Mvc;
using Slot.Model;
using System.Net;

namespace Slot.BackOffice.Data.Responses
{
    /// <summary>
    /// Standard response for forbidden requests.
    /// </summary>
    public class ForbiddenRequestResult : ObjectResult
    {
        /// <summary>
        /// Create a standard response for forbidden requests.
        /// </summary>
        /// <param name="value">Message to be returned.</param>
        /// <param name="errorCode">Error code of type <see cref="ErrorCode"/>.</param>
        public ForbiddenRequestResult(string value, ErrorCode errorCode) : base(errorCode)
        {
            StatusCode = (int)HttpStatusCode.Forbidden;
            Value = new
            {
                Value = value,
                Error = errorCode,
                IsError = true
            };
        }
    }
}
