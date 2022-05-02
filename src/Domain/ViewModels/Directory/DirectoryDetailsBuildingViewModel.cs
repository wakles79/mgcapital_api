using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System.Collections.Generic;

namespace MGCap.Domain.ViewModels.Directory
{
    public class DirectoryDetailsBuildingViewModel : EntityViewModel, IEntityParentViewModel<DirectoryChildBaseViewModel, DirectoryChildBaseViewModel, DirectoryChildBaseViewModel>
    {
        private string _customerName;
        private string _fullAddress;

        public string Name { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get => _customerName.RemoveDuplicatedSpaces(); set => _customerName = value; }

        public string FullAddress { get => _fullAddress.RemoveDuplicatedSpaces(); set => _fullAddress = value; }

        public string EmergencyPhone { get; set; }

        public string EmergencyPhoneExt { get; set; }

        public IList<DirectoryChildBaseViewModel> OperationsManagers => Children1 as IList<DirectoryChildBaseViewModel>;

        public IList<DirectoryChildBaseViewModel> Supervisors => Children2 as IList<DirectoryChildBaseViewModel>;

        public IList<DirectoryChildBaseViewModel> Contacts => Children3 as IList<DirectoryChildBaseViewModel>;

        public IList<DirectoryChildBaseViewModel> Children1 { get; set; }

        public IList<DirectoryChildBaseViewModel> Children2 { get; set; }

        public IList<DirectoryChildBaseViewModel> Children3 { get; set; }
    }
}
