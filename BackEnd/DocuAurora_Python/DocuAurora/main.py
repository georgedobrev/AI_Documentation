import configparser
from pathlib import Path

from flask import Flask

from Services.RabbitMQService import RabbitMQService

from Services.RabbitMQFileConsume import callbackFile
from Services.RabbitMQMessageConsume import callbackMessage


# from Model.t5_model import local_llm

app = Flask(__name__)

if __name__ == '__main__':
    script_dir = Path(__file__).resolve().parent

    # Construct the path to the config file relative to the script's location
    config_file = script_dir / "config.ini"
    rabbitmq_service = RabbitMQService()
    rabbitmq_service.connect()
    queue1_name = 'DocuAurora-Message-Queue'
    queue2_name = 'DocuAurora-File-Queue'
    queue_callbacks = {
        queue1_name: callbackMessage,
        queue2_name: callbackFile
    }

    exchange_name = 'DocuAurora-Exchange'
    routing_key1 = 'DocuAurora-api/RabittMQMessage'
    routing_key2 = 'DocuAurora-api/RabittMQFile'
    rabbitmq_service.create_queues(queue1_name, queue2_name, exchange_name, routing_key1, routing_key2)

    rabbitmq_service.consume_messages(queue_callbacks)

    # Close the connection
    # rabbitmq_service.close_connection()

    # TO DO put this logic in rabittmq when receive msg to send file to model and import s3SERVICE !!!
    # s3_service = S3Service(region_name)
    # s3_service.process_files(bucket_name, object_keys, program_path)

    # TO DO connect this logic T5 tokenizer
    # embedding_service = T5EmbeddingService()
    # document_text = "Your document text here"
    # embedding = embedding_service.get_document_embedding(document_text)
    # print(embedding)
    # print(local_llm('What is the capital of France')) # to do input message
    app.run(host='0.0.0.0', port=5000, debug=True)


