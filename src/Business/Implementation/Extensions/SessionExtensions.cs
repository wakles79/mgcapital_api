// -----------------------------------------------------------------------
// <copyright file="SessionExtensions.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Business.Implementation.Extensions
{
    /// <summary>
    ///     Contains some functionalities for extending
    ///     the <see cref="ISession"/>
    /// </summary>
    public static class SessionExtensions
    {
        // TODO: Robin: comment this method because I'm not sure what it does
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        // TODO: Robin: comment this method because I'm not sure what it does
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
