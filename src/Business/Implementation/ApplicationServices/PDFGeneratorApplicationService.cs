using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class PDFGeneratorApplicationService : IPDFGeneratorApplicationService
    {
        private readonly PDFGeneratorApiOptions _PDFGeneratorApiOptions;

        public PDFGeneratorApplicationService(IOptions<PDFGeneratorApiOptions> opts)
        {
            this._PDFGeneratorApiOptions = opts.Value;
        }

        public async Task<string> GetBase64Document(string documentId, string body)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(this._PDFGeneratorApiOptions.BaseUrl);

                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"templates/{documentId}/output");

                httpRequestMessage.Content = new StringContent(body, Encoding.UTF8, "application/json");

                httpRequestMessage.Headers.Add("Accept", "application/json");
                httpRequestMessage.Headers.Add("X-Auth-Key", this._PDFGeneratorApiOptions.Key);
                httpRequestMessage.Headers.Add("X-Auth-Secret", this._PDFGeneratorApiOptions.Secret);
                httpRequestMessage.Headers.Add("X-Auth-Workspace", this._PDFGeneratorApiOptions.Workspace);

                var response = await httpClient.SendAsync(httpRequestMessage);

                var jsonResult = await response.Content.ReadAsStringAsync();

                JObject jObject = JObject.Parse(jsonResult);

                var base64pdf = jObject["response"].ToString();

                return base64pdf;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<string> GetDocumentUrl(string documentId, string body)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(this._PDFGeneratorApiOptions.BaseUrl);

                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"templates/{documentId}/output?output=url");

                httpRequestMessage.Content = new StringContent(body, Encoding.UTF8, "application/json");

                httpRequestMessage.Headers.Add("Accept", "application/json");
                httpRequestMessage.Headers.Add("X-Auth-Key", this._PDFGeneratorApiOptions.Key);
                httpRequestMessage.Headers.Add("X-Auth-Secret", this._PDFGeneratorApiOptions.Secret);
                httpRequestMessage.Headers.Add("X-Auth-Workspace", this._PDFGeneratorApiOptions.Workspace);

                var response = await httpClient.SendAsync(httpRequestMessage);

                var jsonResult = await response.Content.ReadAsStringAsync();

                JObject jObject = JObject.Parse(jsonResult);

                var documentUrl = jObject["response"].ToString();

                return documentUrl;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
