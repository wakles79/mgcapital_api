// -----------------------------------------------------------------------
// <copyright file="IInspectionsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Calendar;
using MGCap.Domain.ViewModels.Inspection;
using MGCap.Domain.ViewModels.InspectionItem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface IInspectionsRepository : IBaseRepository<Inspection, int>
    {
        Task<DataSource<InspectionGridViewModel>> ReadAllDapperAsync(DataSourceRequestInspection request, int companyId, int? status = -1, int? buildingId = null, int? employeeId = null);
        Task<InspectionReportDetailViewModel> GetInspectionDetailsDapperAsync(int? inspectionlId, Guid? guid);
        Task<IEnumerable<EmailLogEntry>> GetManagersEmailsAsync(int iD);

        Task<DataSource<CalendarGridViewModel>> GetInspectionCalendar(DataSourceRequest request, int companyId);
    }
}