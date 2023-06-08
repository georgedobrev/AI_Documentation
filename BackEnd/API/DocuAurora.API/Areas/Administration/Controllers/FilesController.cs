using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using DocuAurora.Common;
using DocuAurora.Services.Data.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocuAurora.API.Areas.Administration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //TO DO -> Uncomment when done
    // [Authorize(Roles = GlobalConstants.TrainerRoleName)]
    public class FilesController : ControllerBase
    {
        private readonly IS3Service _s3Service;

        public FilesController(IS3Service s3Service)
        {
            this._s3Service = s3Service;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(string key, string bucketName)
        {
            var bucketExists = await this._s3Service.DoesS3BucketExistAsync(bucketName);
            if (!bucketExists)
            {
                return NotFound($"Bucket {bucketName} does not exist.");
            }

            var fileForDownload = await this._s3Service.GetFileAsync(bucketName, key);

            return File(fileForDownload.ResponseStream, fileForDownload.Headers.ContentType);
        }

        // POST api/files
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Post([FromBody] IFormFile file, string bucketName, string? prefix)
        {
            var bucketExists = await this._s3Service.DoesS3BucketExistAsync(bucketName);
            if (!bucketExists)
            {
                return NotFound($"Bucket {bucketName} does not exist.");
            }

            return Ok(await this._s3Service.UploadFileAsync(file, bucketName, prefix));
        }

        // POST api/files
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string key, string bucketName)
        {
            var bucketExists = await this._s3Service.DoesS3BucketExistAsync(bucketName);
            if (!bucketExists)
            {
                return NotFound($"Bucket {bucketName} does not exist.");
            }

            await this._s3Service.DeleteFileAsync(key, bucketName);

            return NoContent();
        }
    }
}
