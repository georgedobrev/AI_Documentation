import json
import os
from pathlib import Path

from Services.LoadDocumentsService import LoadDocumentsService


def callbackFile(ch, method, properties, body):
    data = json.loads(body.decode())
    bucket_name = data['BucketName']
    file_key = data['DocumentNames']
    print(f'Hello Admin, we received your message => {data} commandName => {bucket_name} file_key => {file_key}')
    print(f'file queue {body}')

    main_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    config_file_path = os.path.join(main_dir, 'config.ini')
    load_documents_service = LoadDocumentsService(config_file_path)

    model_dir = os.path.join("BackEnd", "DocuAurora_Python", "DocuAurora", "Model")

    # Get the current working directory
    current_dir = os.getcwd()

    # Construct the absolute path to the "Model" directory
    model_abs_dir = os.path.join(current_dir, model_dir)


    print(f'{bucket_name} -- {file_key} --- {model_abs_dir}')
    load_documents_service.download_files(bucket_name, file_key, model_abs_dir)
    load_documents_service.load_documents(model_abs_dir)

    print(model_abs_dir)
