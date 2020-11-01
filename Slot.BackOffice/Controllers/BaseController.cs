using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Slot.BackOffice.Data.Authentication;
using Slot.Core;
using Slot.Model;
using System.Net;

namespace Slot.BackOffice.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    /// <summary>
    /// Base controller class.
    /// </summary>
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Current <see cref="JwtUser"/> instance available.
        /// </summary>
        public JwtUser JwtUser { get => HttpContext.User == null ? null : new JwtUser(HttpContext.User); }

        /// <summary>
        /// Creates a strongly typed <see cref="Result{TResult, TError}"/> result with the correct <see cref="HttpStatusCode"/>.
        /// </summary>
        /// <typeparam name="TResult">Expected result type.</typeparam>
        /// <typeparam name="TError">Expected error type.</typeparam>
        /// <returns><see cref="Result{TResult, TError}"/> result.</returns>
        protected IActionResult GetResult()
        {
            return Ok(TransformValue());
        }

        /// <summary>
        /// Creates a strongly typed <see cref="Result{TResult, TError}"/> result with the correct <see cref="HttpStatusCode"/>.
        /// </summary>
        /// <typeparam name="TResult">Expected result type.</typeparam>
        /// <typeparam name="TError">Expected error type.</typeparam>
        /// <param name="value">Value for the expected result or error type.</param>
        /// <returns><see cref="Result{TResult, TError}"/> result.</returns>
        protected IActionResult GetResult(object value)
        {
            return Ok(TransformValue(value));
        }

        /// <summary>
        /// Creates a strongly typed <see cref="Result{TResult, TError}"/> result with the correct <see cref="HttpStatusCode"/>.
        /// </summary>
        /// <typeparam name="TResult">Expected result type.</typeparam>
        /// <typeparam name="TError">Expected error type.</typeparam>
        /// <param name="value">Value for the expected result or error type.</param>
        /// <param name="contentType">Content type to be returned.</param>
        /// <returns><see cref="Result{TResult, TError}"/> result.</returns>
        protected IActionResult GetResult(object value, string contentType)
        {
            return Content(JsonConvert.SerializeObject(TransformValue(value)), contentType);
        }

        /// <summary>
        /// Creates a strongly typed <see cref="Result{TResult, TError}"/> result with the correct <see cref="HttpStatusCode"/>.
        /// </summary>
        /// <typeparam name="TResult">Expected result type.</typeparam>
        /// <typeparam name="TError">Expected error type.</typeparam>
        /// <param name="value">Value for the expected result or error type.</param>
        /// <returns><see cref="Result{TResult, TError}"/> result.</returns>
        protected IActionResult GetResult(ErrorCode error)
        {
            return BadRequest(TransformValue(error));
        }

        /// <summary>
        /// Transforms the return value to the standard <see cref="Result{TResult, TError}"/> type.
        /// </summary>
        /// <param name="value">Value to be transformed.</param>
        /// <returns>Transformed value of <see cref="Result{TResult, TError}"/> type.</returns>
        private Result<object, ErrorCode> TransformValue(object value = null)
        {
            return value;
        }
    }
}
