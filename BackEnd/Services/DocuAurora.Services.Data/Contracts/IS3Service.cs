using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data.Contracts
{
    public interface IS3Service
    {
        Task<GetObjectResponse> GetFileAsync(string key, string bucketName = "docu-aurora/Default");

        Task<string> UploadFileAsync(IFormFile file, string bucketName = "docu-aurora/Default",  string? prefix = "DocuAuroraStorage");

        Task<bool> DoesS3BucketExistAsync(string bucketName = "docu-aurora/Default");

        Task<bool> DeleteFileAsync(string key, string bucketName = "docu-aurora/Default");
    }
}
