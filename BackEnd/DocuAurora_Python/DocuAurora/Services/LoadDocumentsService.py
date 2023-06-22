from Services.S3Service import S3Service
from Services.AnswerGeneratorService import AnswerGeneratorService


class LoadDocumentsService:
    def __init__(self, config_file):
        self.config_file = config_file
        self.s3_service = S3Service(config_file)
        self.answer_generator_service = AnswerGeneratorService(config_file)

    def load_documents(self, document_paths):
        self.answer_generator_service.load_documents(document_paths)

    def download_files(self, bucket_name, object_keys, file_path):
        self.s3_service.download_files(bucket_name, object_keys, file_path)

