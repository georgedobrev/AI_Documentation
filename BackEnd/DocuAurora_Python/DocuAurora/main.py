import configparser
from pathlib import Path

from flask import Flask

from Services.RabbitMQService import RabbitMQService
from Services.DocumentStorageService import DocumentStorageService

app = Flask(__name__)

if __name__ == '__main__':
    script_dir = Path(__file__).resolve().parent

    # Construct the path to the config file relative to the script's location
    config_file = script_dir / "config.ini"
    # rabbitmq_service = RabbitMQService(config_file)
    # rabbitmq_service.connect()
    # rabbitmq_service.start_consuming()

    # TO DO put this logic in rabittmq when receive msg to send file to model and import s3SERVICE !!!
    # s3_service = S3Service(region_name)
    # s3_service.process_files(bucket_name, object_keys, program_path)

    # TO DO connect this logic T5 tokenizer
    # embedding_service = T5EmbeddingService()
    # document_text = "Your document text here"
    # embedding = embedding_service.get_document_embedding(document_text)
    # print(embedding)
    doc_storage_service = DocumentStorageService("test-Milcho", config_file , 1)

    # Create an index
    doc_storage_service.create_index()

    # Insert a document
    document_id = "doc1"
    embedding = [0.1, 0.2, 0.3]
    doc_storage_service.insert_document(document_id, embedding)




    app.run(host='0.0.0.0', port=5000, debug=True)
