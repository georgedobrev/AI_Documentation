from flask import Flask
from Services.RabbitMQService import RabbitMQService
from config import host_name, exchange_name, queue_name, message_routing_key, file_routing_key

app = Flask(__name__)

if __name__ == '__main__':
    rabbitmq_service = RabbitMQService(host_name, exchange_name, queue_name, message_routing_key, file_routing_key)
    rabbitmq_service.connect()
    rabbitmq_service.start_consuming()

    # TO DO put this logic in rabittmq when receive msg to send file to model and import s3SERVICE !!!
    # s3_service = S3Service(region_name)
    # s3_service.process_files(bucket_name, object_keys, program_path)

    # TO DO connect this logic T5 tokenizer
    # embedding_service = T5EmbeddingService()
    # document_text = "Your document text here"
    # embedding = embedding_service.get_document_embedding(document_text)
    # print(embedding)

    app.run(host='0.0.0.0', port=5000, debug=True)
