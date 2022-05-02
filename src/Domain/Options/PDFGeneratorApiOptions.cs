using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Options
{
    public class PDFGeneratorApiOptions
    {
        public string BaseUrl { get; set; }

        public string Workspace { get; set; }

        public string Key { get; set; }

        public string Secret { get; set; }
    }
}
