using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Entities
{
    public interface IFranchiseEntity
    {
        /// <summary>
        ///     Gets or sets the Franchise related PK
        /// </summary>
        int? FranchiseId { get; set; }

        Franchise Franchise { get; set; }
    }
}
