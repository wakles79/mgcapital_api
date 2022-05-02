// -----------------------------------------------------------------------
// <copyright file="ICleaningReportItemAttachmentsRepository.cs" company="Axzes">
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
    public interface ICleaningReportItemAttachmentsRepository: IBaseRepository<CleaningReportItemAttachment,int>
    {
    }
}
