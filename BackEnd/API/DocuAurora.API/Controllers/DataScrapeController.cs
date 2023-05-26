using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace DocuAurora.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataScrapeController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> ExtractTextFromPdf(IFormFile pdfFile)
        {
            using var httpClient = new HttpClient();

            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(pdfFile.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(pdfFile.ContentType);
            fileContent.Headers.ContentLength = pdfFile.Length;

            content.Add(fileContent, "pdf_file", pdfFile.FileName);

            var response = await httpClient.PostAsync("http://127.0.0.1:5000/api/extract_text", content); // put your host
            var data = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<string[]>(data);

            return Ok(result);
        }
    }
}