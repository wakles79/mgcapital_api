/* 
 *  File: IAddressesApplicationService.cs
 *  Copyright (c) 2019 robin
 *  
 *  This source code is Copyright Axzes and MAY NOT be copied, reproduced,
 *  published, distributed or transmitted to or stored in any manner without prior
 *  written consent from Axzes (https://www.axzes.com).
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using MGCap.Domain.Models;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IAddressesApplicationService : IBaseApplicationService<Address, int>
    {
        Task<IEnumerable<Address>> GeodecodeAllAsync();
    }
}