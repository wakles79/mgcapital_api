// -----------------------------------------------------------------------
// <copyright file="IInspectionNotesRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------


using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Inspection;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface IInspectionNotesRepository : IBaseRepository<InspectionNote, int>
    {
        Task<DataSource<InspectionNoteGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int inspectionID);
    }
}
