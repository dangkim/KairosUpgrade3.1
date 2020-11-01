using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Slot.Model.Entity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Slot.BackOffice.Data.Authentication
{
    /// <summary>
    /// Class wrapper for jwt details.
    /// </summary>
    public class JwtUser
    {
        /// <summary>
        /// Instantiates a new instance of the wrapper for the jwt details.
        /// </summary>
        /// <param name="principal"><see cref="ClaimsPrincipal"/> instance containing user details.</param>
        public JwtUser(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("Principal must not be null.");
            }

            if (!principal.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Principal must be authenticated.");
            }

            UserId = Convert.ToInt32(principal.Claims.FirstOrDefault(claim => claim.Type == BackOfficeClaimTypes.UserId)?.Value);
            Username = principal.Claims.FirstOrDefault(claim => claim.Type == BackOfficeClaimTypes.Username)?.Value;
            RoleId = Convert.ToInt32(principal.Claims.FirstOrDefault(claim => claim.Type == BackOfficeClaimTypes.RoleId)?.Value);
            Role = Enum.GetName(typeof(Roles), RoleId);
            RoleName = principal.Claims.FirstOrDefault(claim => claim.Type == BackOfficeClaimTypes.RoleName)?.Value;
            RoleEnum = Enum.Parse<Roles>(Convert.ToString(RoleId));
            OperatorId = Convert.ToInt32(principal.Claims.FirstOrDefault(claim => claim.Type == BackOfficeClaimTypes.OperatorId)?.Value);
            Operator = principal.Claims.FirstOrDefault(claim => claim.Type == BackOfficeClaimTypes.Operator)?.Value;
            Principal = principal;
        }

        /// <summary>
        /// Instantiates a new instance of the wrapper for the jwt details.
        /// </summary>
        /// <param name="account"><see cref="Account"/> instance containing user details.</param>
        public JwtUser(Account account)
        {
            if (account == null)
            {
                throw new ArgumentNullException("Account must not be null");
            }

            UserId = Convert.ToInt32(account.Id);
            Username = account.Username;
            RoleId = account.RoleId;
            Role = Enum.GetName(typeof(Roles), RoleId);
            RoleName = account.Role.Name;
            RoleEnum = Enum.Parse<Roles>(Convert.ToString(RoleId));
            OperatorId = account.OperatorId;
            Operator = account.Operator.Tag;

            var identity = new GenericIdentity(Username, JwtBearerDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Role, Role));
            identity.AddClaims(Claims);
            Principal = new ClaimsPrincipal(identity);
        }

        private int? userId;
        public int? UserId
        {
            get => userId;
            set => userId = value == 0 ? null : value;
        }

        public string Username { get; }

        private int? roleId;
        public int? RoleId
        {
            get => roleId;
            set => roleId = value == 0 ? null : value;
        }


        /// <summary>
        /// Used for determining role in the backend using the <see cref="Roles"/> type name.
        /// </summary>
        public string Role { get; }

        public Roles RoleEnum { get; }

        public string RoleName { get; }

        private int? operatorId;
        public int? OperatorId
        {
            get => operatorId;
            set => operatorId = value == 0 ? null : value;
        }

        public string Operator { get; }

        /// <summary>
        /// Principal used for identifying the current user.
        /// </summary>
        public ClaimsPrincipal Principal { get; }

        /// <summary>
        /// Claims required for token based on the user's current information.
        /// </summary>
        public Claim[] Claims
        {
            get => new Claim[]
            {
                new Claim(BackOfficeClaimTypes.UserId, Convert.ToString(UserId)),
                new Claim(BackOfficeClaimTypes.Username, Username),
                new Claim(BackOfficeClaimTypes.RoleId, Convert.ToString(RoleId)),
                new Claim(BackOfficeClaimTypes.RoleName, Role),
                new Claim(BackOfficeClaimTypes.Role, Convert.ToString(RoleEnum)),
                new Claim(BackOfficeClaimTypes.OperatorId, Convert.ToString(OperatorId)),
                new Claim(BackOfficeClaimTypes.Operator, Operator)
            };
        }

        /// <summary>
        /// Gets a jwt token based on the user's current information.
        /// </summary>
        /// <param name="securityKey">JWT security key.</param>
        /// <param name="issuer">JWT issuer.</param>
        /// <param name="audience">JWT audience.</param>
        /// <returns>JWT Token</returns>
        public string GetToken(string securityKey, string issuer, string audience, double tokenDuration)
        {
            var token = GenerateToken(securityKey, issuer, audience, tokenDuration, Claims);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Gets a jwt token based on the user's current information.
        /// </summary>
        /// <param name="securityKey">JWT security key.</param>
        /// <param name="issuer">JWT issuer.</param>
        /// <param name="audience">JWT audience.</param>
        /// <returns>JWT Token</returns>
        //public string GetTokenForCache(string securityKey, string issuer, string audience, double tokenDuration)
        //{
        //    //var claims = new Claim[] {
        //    //    new Claim(ClaimTypes.Role, "Administrator")
        //    //};

        //    var token = GenerateToken(securityKey, issuer, audience, tokenDuration, Claims);

        //    var tokenHandler = new JwtSecurityTokenHandler();

        //    return tokenHandler.WriteToken(token);
        //}

        private JwtSecurityToken GenerateToken(string securityKey, string issuer, string audience, double tokenDuration, Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenDuration),
                signingCredentials: credentials
            );

            return token;
        }
    }
}
