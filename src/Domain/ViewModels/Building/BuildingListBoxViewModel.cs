using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;


namespace MGCap.Domain.ViewModels.Building
{
    public class BuildingListBoxViewModel : ListBoxViewModel
    {
        private string _fullAddress;

        public string FullAddress
        {
            get => _fullAddress.RemoveDuplicatedSpaces();
            set => _fullAddress = value;
        }

        public string Code;
    }

    public class BuildingByOperationsManagerListBoxViewModel : BuildingListBoxViewModel
    {
        public bool IsSupervisor { get; set; }

        public bool IsTemporaryOperationsManager { get; set; }

        public string OperationsManagerFullName
        {
            get => _operationsManagerFullName.RemoveDuplicatedSpaces();
            set => _operationsManagerFullName = value;
        }
        private string _operationsManagerFullName;
    }
}
