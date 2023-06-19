import json
import os
from pathlib import Path

from Services.LoadDocumentsService import LoadDocumentsService


def callbackFile(ch, method, properties, body, publish_message):
    data = json.loads(body.decode())
    bucket_name = data['BucketName']
    file_key = data['DocumentNames']
    print(f'Hello Admin, we received your message => {data} commandName => {bucket_name} file_key => {file_key}')
    print(f'file queue {body}')

    publish_message("primerno")

    # to do return messages. how to write public message

    script_dir = Path(__file__).resolve().parent
    config_file = script_dir / "config.ini"
    load_documents_service = LoadDocumentsService(config_file)
    base_path = os.path.dirname(os.path.abspath(__file__))
    file_path = os.path.join(base_path, 'AI_Documentation', 'BackEnd', 'DocuAurora_Python', 'DocuAurora', 'Model')

    load_documents_service.download_files(bucket_name, file_key, file_path)
    load_documents_service.unzip_files(file_path, file_path)
    load_documents_service.load_documents(file_path)
    print(file_path)
