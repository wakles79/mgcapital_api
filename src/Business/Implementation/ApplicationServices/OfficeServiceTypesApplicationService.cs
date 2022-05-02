// -----------------------------------------------------------------------
// <copyright file="OfficeServiceTypesApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.OfficeServiceType;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class OfficeServiceTypesApplicationService : BaseSessionApplicationService<OfficeServiceType, int>, IOfficeServiceTypesApplicationService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OfficeTypesApplicationService"/> class.
        /// </summary>
        /// <param name="repository">
        ///     To inject the implementation of <see cref="OfficeTypesRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param>
        /// <param name="httpContextAccessor">
        ///     To access data in the current <see cref="HttpContext"/>
        /// </param>
        /// <param name="userResolverService">For getting some data from the current User</param>
        public OfficeServiceTypesApplicationService(
            IOfficeServiceTypesRepository repository,
            IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        /// <summary>
        ///     Gets the object that contains the operations of
        ///     the DataAcces layer
        /// </summary>
        public new IOfficeServiceTypesRepository Repository => base.Repository as IOfficeServiceTypesRepository;

        public Task<DataSource<OfficeServiceTypeListViewModel>> ReadAllCboDapperAsync(int status = -1, int rateType = -1, string exclude = "")
        {
            return this.Repository.ReadAllCboDapperAsync(this.CompanyId, status, rateType, exclude);
        }

        public Task<DataSource<OfficeServiceTypeGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? isEnabled = -1)
        {
            return this.Repository.ReadAllDapperAsync(request, this.CompanyId, isEnabled);
        }
    }
}