// -----------------------------------------------------------------------
// <copyright file="DependenciesRegistry.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using StructureMap;
using StructureMap.Graph.Scanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Configuration
{
    /// <summary>
    ///     Contiains the funcionalities for
    ///     configuring StructuredMap as Dependency
    ///     resolver.
    /// </summary>
    public class DependenciesRegistry : Registry
    {
        public DependenciesRegistry()
        {
            this.Scan(_ =>
            {
                _.WithDefaultConventions();

                _.AddAllTypesOf(typeof(IBaseRepository<,>));

                _.AddAllTypesOf(typeof(IBaseApplicationService<,>));
            });

            TypeRepository.AssertNoTypeScanningFailures();
        }
    }
}
