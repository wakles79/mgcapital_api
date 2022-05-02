using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IAzureStorage
    {
        Task<Tuple<string, string, DateTime>> UploadImageAsync(
            Stream file,
            string containerName = "img",
            string prevBlobName = null,
            string ext = "jpg",
            string contentType = "image/jpeg");

        Task<Tuple<string, string, DateTime>> UploadFileAsync(Stream file, string ext, string contentType);

        Task<bool> DeleteImageAsync(string containerName = "img", string blobName = null);
    }
}
