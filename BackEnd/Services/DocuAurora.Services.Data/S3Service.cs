using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using DocuAurora.Services.Data.Contracts;
using Microsoft.AspNetCore.Http;
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

        public S3Service(IAmazonS3 s3Client)
        {
            this._s3Client = s3Client;
        }

        public async Task<GetObjectResponse> GetFileAsync(string bucketName, string key)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            var response = await this._s3Client.GetObjectAsync(request);
            return response;
        }

        public async Task<string> UploadFileAsync(string bucketName, IFormFile file, string? prefix)
        {
            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}",
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);

            await this._s3Client.PutObjectAsync(request);

            return $"File {prefix}/{file.FileName} uploaded to S3 successfully!";

        }

        public async Task<bool> DoesS3BucketExistAsync(string bucketName)
        {
            return await this._s3Client.DoesS3BucketExistAsync(bucketName);
        }


        //public async Task<bool> DeleteFileAsync(string bucketName, string key)
        //{
        //    var deleteRequest = new DeleteObjectRequest
        //    {
        //        BucketName = bucketName,
        //        Key = key
        //    };

        //    var response = await _s3Client.DeleteObjectAsync(deleteRequest);
        //    return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
        //}
    }
}
