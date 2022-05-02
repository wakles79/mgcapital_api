// -----------------------------------------------------------------------
// <copyright file="IBaseEntityWithAttachmentsService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    /// <summary>
    ///     Contains the main functionalities
    ///     for handling operations related with
    ///     the User's session and the entity attachments
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for manipulate</typeparam>
    /// <typeparam name="TKey">The type of the key for that entity</typeparam>
    public interface IBaseEntityWithAttachmentsService<TEntity, TKey> : IDisposable
        where TEntity : Entity<TKey>
    {
        #region Attachments

        Task<TEntity> AddAttachmentAsync(TEntity obj);


        Task<TEntity> UpdateAttachmentsAsync(TEntity obj);


        Task<TEntity> GetAttachmentAsync(Func<TEntity, bool> filter);


        Task RemoveAttachmentsAsync(TKey objId);


        #endregion Attachments
    }
}
