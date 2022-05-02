using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Business.Implementation.Services
{
    public interface IUserResolverService
    {
        /// <summary>
        ///     Get the UserName of the current logged User.
        /// </summary>
        /// <returns>The UserName of the user; unknown if there isn't a logged User.</returns>
        string GetUser();
    }
}
