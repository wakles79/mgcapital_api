using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IPDFGeneratorApplicationService 
    {
        Task<string> GetBase64Document(string documentId, string body);

        Task<string> GetDocumentUrl(string documentId, string body);
    }
}
