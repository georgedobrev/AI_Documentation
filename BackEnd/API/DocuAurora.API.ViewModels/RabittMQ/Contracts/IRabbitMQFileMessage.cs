using System;
namespace DocuAurora.API.ViewModels.RabittMQ.Contracts
{
	public interface IRabbitMQFileMessage<T>
	{
        public string BucketName { get; set; }

		public T DocumentNames { get; set; }
	}
}
