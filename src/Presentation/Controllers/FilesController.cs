using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Utils;
using MGCap.Presentation.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    public class FilesController : Controller
    {
        protected readonly IAzureStorage _azureStorage;

        public FilesController(IAzureStorage azureStorage)
        {
            _azureStorage = azureStorage;
        }

        #region Attachments

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UploadAttachments([FromForm] string azureStorageDirectory = "img")
        {
            IFormFileCollection files = Request.Form.Files;

            if (files.Count == 0)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "The provided files collection can not be empty!");
            }

            List<string> notUploaded = new List<string>();
            List<ImageUploadViewModel> uploaded = new List<ImageUploadViewModel>();

            try
            {
                foreach (IFormFile file in files)
                {
                    if (file.Length > 0)
                    {
                        Stream imageFile = file.OpenReadStream();
                        if (imageFile != null)
                        {
                            // Tuple<string, string, DateTime>
                            var result = await _azureStorage.UploadImageAsync(imageFile, azureStorageDirectory, contentType: file.ContentType);

                            uploaded.Add(new ImageUploadViewModel
                            {
                                FileName = file.FileName,
                                BlobName = result.Item1,
                                FullUrl = result.Item2,
                                ImageTakenDate = result.Item3
                            });
                        }
                        else
                        {
                            notUploaded.Add(file.FileName);
                        }
                    }
                    else
                    {
                        notUploaded.Add(file.FileName);
                    }
                }

                if (notUploaded.Count.Equals(0))
                {
                    return new JsonResult(uploaded);
                }

                if (notUploaded.Count < files.Count)
                {
                    return StatusCode(
                        (int)HttpStatusCode.PartialContent,
                        string.Format("The following files were not uploaded: {0}", string.Join(", ", notUploaded))
                    );
                }

                return StatusCode((int)HttpStatusCode.NoContent, "Any file was uploaded!");
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                if (notUploaded.Count < files.Count)
                {
                    return StatusCode(
                        (int)HttpStatusCode.PartialContent,
                        string.Format("The following images did not get uploaded: {0}", string.Join(", ", notUploaded))
                    );
                }

                return StatusCode((int)HttpStatusCode.NoContent, "Any file was uploaded!");
            }
        }

        [HttpDelete]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteAttachmentByBlobName(string blobName)
        {
            try
            {
                // TODO: the container may change, take this in consideration
                bool result = await _azureStorage.DeleteImageAsync(blobName: blobName);
#if DEBUG
                Console.WriteLine($"azure delete result: {result}");
#endif
            }
            catch (Exception)
            {
                // It does not matter, let the file remains in the storage. 
                // The most important thing is to remove it from DB
            }

            return Ok();
        }

        #endregion Attachments
    }
}
