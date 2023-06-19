import configparser
import os

import subprocess
import boto3
from Model.t5_model import setup_model, load_documents, setup_pinecone, setup_retrieval_qa, split_text, ask_question
from S3Service import S3Service
from AnswerGeneratorService import AnswerGeneratorService


class LoadDocumentsService:
    def __init__(self, config_file):
        self.config_file = config_file
        self.s3_service = S3Service(config_file)
        self.answer_generator_service = AnswerGeneratorService(config_file)

    def load_documents(self, document_paths):
        self.answer_generator_service.load_documents(document_paths)

    def download_files(self, bucket_name, object_keys, file_path):
        self.s3_service.download_files(bucket_name, object_keys, file_path)

    def unzip_files(self, file_path, extract_path):
        self.s3_service.unzip_files(file_path, extract_path)
