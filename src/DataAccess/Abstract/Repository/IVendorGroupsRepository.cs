// ----------------------------------------------------------------------- 
// <copyright file="IVendorGroupsRepository.cs" company="Axzes"> 
// This source code is Copyright Axzes and MAY NOT be copied, reproduced, 
// published, distributed or transmitted to or stored in any manner without prior 
// written consent from Axzes (https://www.axzes.com). 
// </copyright> 
// ----------------------------------------------------------------------- 
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.DataAccess.Abstract.Repository
{
    /// <summary> 
    ///     Contains the declaration of the functionalities 
    ///     for handling operations on the <see cref="VendorGroup"/> 
    /// </summary> 
    public interface IVendorGroupsRepository : IBaseRepository<VendorGroup,int>
    {
    }
}
