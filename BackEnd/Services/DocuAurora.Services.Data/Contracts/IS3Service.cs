using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data.Contracts
{
    public interface IS3Service
    {
        Task<GetObjectResponse> GetFileAsync(string bucketName, string key);

        Task<string> UploadFileAsync(string bucketName, string? prefix, IFormFile file);

        Task<bool> DoesS3BucketExistAsync(string bucketName);

        Task<bool> DeleteFileAsync(string bucketName, string key);

        Task<List<string>> GetAllFilesFromBucket(string bucketName);

    }
}
