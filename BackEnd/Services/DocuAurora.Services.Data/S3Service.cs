using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using DocuAurora.Services.Data.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;

        public S3Service(
            IAmazonS3 s3Client,
            IConfiguration configuration)
        {
            this._s3Client = s3Client;
            this._configuration = configuration;
        }

        public async Task<GetObjectResponse> GetFileAsync(string bucketName, string key)
        {
            if (string.IsNullOrEmpty(bucketName) || string.IsNullOrWhiteSpace(bucketName))
            {
                bucketName = this._configuration["AWSBucketName"];
            }

            var response = await this._s3Client.GetObjectAsync(bucketName, key);
            return response;
        }

        public async Task<string> UploadFileAsync(string bucketName, string? prefix, IFormFile file)
        {
            if (string.IsNullOrEmpty(bucketName) || string.IsNullOrWhiteSpace(bucketName))
            {
                bucketName = this._configuration["AWSBucketName"];
            }

            if (string.IsNullOrEmpty(prefix) || string.IsNullOrWhiteSpace(prefix))
            {
                prefix = this._configuration["AWSPrefix"];
            }

            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = $"{prefix}/{file.FileName}",
                InputStream = file.OpenReadStream(),
            };
            request.Metadata.Add("Content-Type", file.ContentType);

            await this._s3Client.PutObjectAsync(request);

            return $"File {prefix}/{file.FileName} uploaded to S3 successfully!";
        }

        public async Task<bool> DoesS3BucketExistAsync(string bucketName)
        {
            if (string.IsNullOrEmpty(bucketName) || string.IsNullOrWhiteSpace(bucketName))
            {
                bucketName = this._configuration["AWSBucketName"];
            }

            return await this._s3Client.DoesS3BucketExistAsync(bucketName);
        }

        public async Task<bool> DeleteFileAsync(string bucketName, string key)
        {
            var response = await this._s3Client.DeleteObjectAsync(bucketName, key);
            return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
        }

        public async Task<List<string>> GetAllFilesFromBucket(string bucketName)
        {
            if (string.IsNullOrEmpty(bucketName) || string.IsNullOrWhiteSpace(bucketName))
            {
                bucketName = this._configuration["AWSBucketName"];
            }

            List<string> objectKeys = new List<string>();

            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = bucketName
            };

            ListObjectsV2Response response = await this._s3Client.ListObjectsV2Async(request);

            foreach (S3Object entry in response.S3Objects)
            {
                objectKeys.Add(entry.Key);
            }

            return objectKeys;
        }
    }
}
