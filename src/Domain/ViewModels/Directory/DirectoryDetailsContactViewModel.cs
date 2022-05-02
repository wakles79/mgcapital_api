using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Directory
{
    public class DirectoryDetailsContactViewModel :EntityViewModel, IEntityParentViewModel<DirectoryChildBaseViewModel, 
                                                                                           DirectoryChildBaseViewModel, 
                                                                                           DirectoryChildBaseViewModel, 
                                                                                           DirectoryChildBaseViewModel>
    {
        public string Name { get; set; }

        public DateTime Birthday { get; set; }

        public string Notes { get; set; }

        public IList<DirectoryChildBaseViewModel> Phones => Children1 as IList<DirectoryChildBaseViewModel>;

        public IList<DirectoryChildBaseViewModel> Emails => Children2 as IList<DirectoryChildBaseViewModel>;

        public IList<DirectoryChildBaseViewModel> Buildings => Children3 as IList<DirectoryChildBaseViewModel>;

        public IList<DirectoryChildBaseViewModel> Customers => Children4 as IList<DirectoryChildBaseViewModel>;


        public IList<DirectoryChildBaseViewModel> Children4 { get; set; }

        public IList<DirectoryChildBaseViewModel> Children3 { get; set; }

        public IList<DirectoryChildBaseViewModel> Children2 { get; set; }

        public IList<DirectoryChildBaseViewModel> Children1 { get; set; }
    }
}
