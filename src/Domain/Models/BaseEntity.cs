// <copyright file="BaseEntity.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Models
{
    /// <summary>
    ///     Base abstraction of every entity.
    /// </summary>
    public abstract class BaseEntity
    {

        public virtual void BeforeCreate(string userEmail = "", int companyId = -1)
        {
            return;
        }

        public virtual void BeforeUpdate(string userEmail = "")
        {
            return;
        }
    }
}
