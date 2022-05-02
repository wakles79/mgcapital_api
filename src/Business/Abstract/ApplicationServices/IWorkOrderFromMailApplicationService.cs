using MGCap.Domain.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IWorkOrderFromMailApplicationService
    {
        Task<int> ReadInbox();
    }
}
