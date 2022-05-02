using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Account;
using MGCap.Domain.ViewModels.CustomerUser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface ICustomerUserService : IBaseApplicationService<CustomerUser, int>
    {
        Task<CustomerContactExistsViewModel> GetCustomerContactByEmail(string email);

        Task<CustomerLoginResponseViewModel> GetWithCompanyDapperAsync(string userEmail);
    }
}
