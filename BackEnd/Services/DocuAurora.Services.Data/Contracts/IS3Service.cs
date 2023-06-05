using System;
using System.IO;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data.Contracts
{
	public interface IS3Service
	{
        Task<Stream> GetFileAsync(string bucketName, string key);

    }
}

