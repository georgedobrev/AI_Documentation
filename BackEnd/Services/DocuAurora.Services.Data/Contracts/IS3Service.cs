using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data.Contracts
{
    public interface IS3Service
    {
        //  Task<Stream> GetFileAsync(string bucketName, string key);

        Task<string> UploadFileAsync(string bucketName, IFormFile file, string? prefix = "DocuAuroraStorage");

        //  Task<bool> DeleteFileAsync(string bucketName, string key);
    }
}
