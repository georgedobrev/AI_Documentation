using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using DocuAurora.API.ViewModels.RabittMQ;
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
        private readonly IRabbitMQService _rabbitMQService;
        private readonly RabbitMQFileKeyMessage _rabbitMQFileKeyMessage;

        public FilesController(
            IS3Service s3Service,
            IRabbitMQService rabbitMQService,
            RabbitMQFileKeyMessage rabbitMQFileKeyMessage
           )
        {
            this._s3Service = s3Service;
            this._rabbitMQService = rabbitMQService;
            this._rabbitMQFileKeyMessage = rabbitMQFileKeyMessage;
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
        //[ProducesResponseType(200)]
        //[ProducesResponseType(401)]
        //[ProducesResponseType(404)]
        public async Task<IActionResult> Post(IFormFile file)
        {
            var bucketExists = await this._s3Service.DoesS3BucketExistAsync();
            if (!bucketExists)
            {
               return NotFound($"Bucket does not exist.");
            }

            var key = await this._s3Service.UploadFileAsync(file);

            this._rabbitMQFileKeyMessage.CommandName = "Post";
            this._rabbitMQFileKeyMessage.Payload = key;

            this._rabbitMQService.SendMessage(this._rabbitMQFileKeyMessage);

            return Ok();
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
