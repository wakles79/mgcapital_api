// -----------------------------------------------------------------------
// <copyright file="ContactListboxViewModel.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------


using MGCap.Domain.ViewModels.Common;

namespace MGCap.Domain.ViewModels.Contact
{
    public class ContactListBoxViewModel : ListBoxViewModel
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullAddress { get; set; }

        public string FullName => Name;
    }
}