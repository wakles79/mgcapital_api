using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.Common
{
    // Response of 'UploadImage' endpoint
    public class ImageUploadViewModel
    {
        public string FileName { get; set; }

        public string BlobName { get; set; }

        public string FullUrl { get; set; }

        public DateTime ImageTakenDate { get; set; }
    }
}
