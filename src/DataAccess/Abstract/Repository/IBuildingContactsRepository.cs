// -----------------------------------------------------------------------
// <copyright file="IBuildingContactsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;

namespace MGCap.DataAccess.Abstract.Repository
{
    /// <summary>
    ///     Contains the declaration of the base
    ///     functionalities for the repositories
    /// </summary>
    /// <typeparam name="TEntity">
    ///     The type of entity that the actual implementation
    ///     of this interface handles
    /// </typeparam>
    /// <typeparam name="TKey">
    ///     The type of the <typeparamref name="TEntity"/>'s Primary Key
    /// </typeparam>
    public interface IBuildingContactsRepository
    {
        /// <summary>
        ///     Gets the <see cref="DbSet{BuildingContact}"/> to perform actions on the
        ///     table that the <typeparamref name="BuildingContact"/>
        ///     represents
        /// </summary>

        DbSet<BuildingContact> Entities { get; }

        BuildingContact Add(BuildingContact obj);

        Task<BuildingContact> AddAsync(BuildingContact obj);

        IQueryable<BuildingContact> ReadAll(Func<BuildingContact, bool> filter);

        Task<IQueryable<BuildingContact>> ReadAllAsync(Func<BuildingContact, bool> filter);

        void Remove(Func<BuildingContact, bool> filter);

        Task<BuildingContact> UpdateAsync(BuildingContact obj);

        bool Exists(int contactId, int addressId);

        Task<BuildingContact> SingleOrDefaultAsync(Func<BuildingContact, bool> filter);

        void Remove(BuildingContact obj);

        Task<DataSource<ContactGridViewModel>> ReadAllAsyncDapper(DataSourceRequest request, int buildingId);

        Task<DataSource<ContactGridViewModel>> ReadAllByBuildingIdsAsyncDapper(DataSourceRequest request, IEnumerable<int> buildingIds);

        Task<DataSource<ListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int companyId, int? id, int? buildingId, WorkOrderContactType type = null);
    }
}
