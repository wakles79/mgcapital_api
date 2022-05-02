using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Account;
using MGCap.Domain.ViewModels.CustomerUser;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class CustomerUserService : BaseApplicationService<CustomerUser, int>, ICustomerUserService
    {
        public new ICustomerUserRepository Repository => base.Repository as ICustomerUserRepository;

        public CustomerUserService(ICustomerUserRepository repository) : base(repository)
        {
        }
        
        public async Task<CustomerContactExistsViewModel> GetCustomerContactByEmail(string email)
        {
            var result = await Repository.GetCustomerContactByEmail(email);
            return result;
        }

        public async Task<CustomerLoginResponseViewModel> GetWithCompanyDapperAsync(string userEmail)
        {
            var result = await Repository.GetWithCompanyDapperAsync(userEmail);
            return result;
        }
    }
}
