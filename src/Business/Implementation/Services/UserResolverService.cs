using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Business.Implementation.Services
{
    /// <summary>
    ///     Service to obtain logged user
    /// </summary>
    public class UserResolverService : IUserResolverService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserResolverService"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The accessor of the environment <see cref="HttpContext"/></param>
        public UserResolverService(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        ///     Get the UserName of the current logged User.
        /// </summary>
        /// <returns>The UserName of the user; unknown if there isn't a logged User.</returns>
        public string GetUser()
        {
            var username = this._httpContextAccessor
                            ?.HttpContext
                            ?.User
                            ?.Identity
                            ?.Name;

            return username ?? "Unknown";
        }
    }
}
