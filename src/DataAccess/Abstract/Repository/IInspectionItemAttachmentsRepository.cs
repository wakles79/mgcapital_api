// -----------------------------------------------------------------------
// <copyright file="IInspectionItemAttachmentsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.InspectionItem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface IInspectionItemAttachmentsRepository : IBaseRepository<InspectionItemAttachment, int>
    {
        Task<DataSource<InspectionItemAttachmentBaseViewModel>> ReadAllDapperAsync(DataSourceRequest request, int inspectionItemId);
    }
}
