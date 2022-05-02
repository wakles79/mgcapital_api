// -----------------------------------------------------------------------
// <copyright file="EntityViewModel.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------


using Dapper;

namespace MGCap.Domain.ViewModels.Common
{

    public abstract class EntityViewModel<TKey> : IEntityViewModel<TKey>
    {
        public TKey ID { get; set; }
    }

    /// <summary>
    ///     Base class that encapsulates all common VM properties
    /// </summary>
    public class EntityViewModel : IEntityViewModel
    {
        public int ID { get; set; }
    }

    public interface IEntityViewModel<TKey>
    {
        TKey ID { get; set; }
    }

    public interface IEntityViewModel : IEntityViewModel<int>
    {

    }
}
