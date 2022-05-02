using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Contact;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Directory
{
    public class DirectoryDetailsCustomerViewModel : EntityViewModel, IEntityParentViewModel<DirectoryEntityBaseViewModel, DirectoryChildBaseViewModel>
    {
        public string Name { get; set; }
        
        public string Notes { get; set; }
        
        public IList<DirectoryEntityBaseViewModel> Buildings => Children1 as IList<DirectoryEntityBaseViewModel>;

        public IList<DirectoryChildBaseViewModel> Contacts => Children2 as IList<DirectoryChildBaseViewModel>;

        public IList<DirectoryChildBaseViewModel> Children2 { get; set; }

        public IList<DirectoryEntityBaseViewModel> Children1 { get; set; }
    }
}
