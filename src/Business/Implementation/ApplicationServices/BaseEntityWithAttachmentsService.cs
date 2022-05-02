// -----------------------------------------------------------------------
// <copyright file="BaseEntityWithAttachmentsService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Repositories;
using MGCap.Domain.Entities;
using MGCap.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    /// <summary>
    ///     Contains the main functionalities
    ///     for handling operations related with
    ///     the User's session and the entity attachments
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that the class with handle</typeparam>
    /// <typeparam name="TEntityAttachments">The type of the entity that handle the attachments</typeparam>
    /// <typeparam name="TKey">The type of the PK of the handled Entity and </typeparam>
    public abstract class BaseEntityWithAttachmentsService<TEntity, TEntityAttachments, TKey> : BaseSessionApplicationService<TEntity, TKey>, IBaseEntityWithAttachmentsService<TEntityAttachments, TKey>
        where TEntity : AuditableCompanyEntity<TKey>
        where TEntityAttachments : AuditableEntity<TKey>
    {

        /// <summary>
        /// Gets or Sets the Repository for handling the attachment entity object
        /// </summary>
        public IBaseRepository<TEntityAttachments, TKey> AttachmentsRepository { get; set; }

        protected readonly IAzureStorage AzureStorage;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseEntityWithAttachmentsService{TEntity, TEntityAttachments, TKey}"/> class.
        /// </summary>
        /// <param name="repository">Inject an <see cref="IBaseRepository{TEntity,TKey}"/> instance</param>
        /// <param name="attachmentsRepository">Inject an <see cref="IBaseRepository{TEntity,TKey}"/> instance</param>
        /// <param name="httpContextAccessor">Inject an <see cref="IHttpContextAccessor"/> for accessing values in the <see cref="HttpContext"/></param>
        protected BaseEntityWithAttachmentsService(
            IAzureStorage azureStorage,
            IBaseRepository<TEntity, TKey> repository,
            IBaseRepository<TEntityAttachments, TKey> attachmentsRepository,
            IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
            this.AttachmentsRepository = attachmentsRepository;
            this.AzureStorage = azureStorage;
        }

        #region Attachments

        public async Task<TEntityAttachments> AddAttachmentAsync(TEntityAttachments obj)
        {
            return await this.AttachmentsRepository.AddAsync(obj);
        }

        public async Task<TEntityAttachments> UpdateAttachmentsAsync(TEntityAttachments obj)
        {
            return await this.AttachmentsRepository.UpdateAsync(obj);
        }

        public async Task<TEntityAttachments> GetAttachmentAsync(Func<TEntityAttachments, bool> filter)
        {
            return await this.AttachmentsRepository.SingleOrDefaultAsync(filter);
        }

        public async Task RemoveAttachmentsAsync(TKey objId)
        {
            var obj = await this.AttachmentsRepository.SingleOrDefaultAsync(a => a.ID.Equals(objId));
            if (obj == null)
            {
                throw new Exception($"Error deleting attachments");
            }

            try
            {
                var blob = obj as Attachment;
                // TODO: the container may change, take this in consideration
                bool result = await AzureStorage.DeleteImageAsync(blobName: blob.BlobName);
#if DEBUG
                Console.WriteLine($"azure delete result: {result}");
#endif
            }
            catch (Exception)
            {
                // It does not matter, let the file remains in the storage. 
                // The most important thing is to remove it from DB
            }
            await this.AttachmentsRepository.RemoveAsync(objId);
        }

        #endregion Attachments
    }
}