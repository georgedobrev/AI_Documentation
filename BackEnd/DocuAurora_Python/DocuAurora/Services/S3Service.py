import os
import boto3
import subprocess
import configparser
import zipfile


class S3Service:
    def __init__(self, config_file):
        self.config_file = config_file
        self.region_name = self._get_config_value('S3', 'region_name')
        # aws_access_key_id = os.environ.get('AWS_ACCESS_KEY_ID')
        # aws_secret_access_key = os.environ.get('AWS_SECRET_ACCESS_KEY')

        self.s3 = boto3.client('s3', region_name=self.region_name)

    def _get_config_value(self, section, key):
        config = configparser.ConfigParser()
        config.read(self.config_file)
        return config.get(section, key)

    def download_files(self, bucket_name, object_keys, file_path):
        for object_key in object_keys:
            # directory_path = r'C:\Users\Milcho\OneDrive\Desktop\Blank\AI_Documentation\BackEnd\DocuAurora_Python\DocuAurora\Model'

            # print(directory_path)
            print(file_path)

            # file_path = os.path.join( directory_path, object_key)
            file_path = os.path.join( file_path, object_key)

            print(file_path)
            self.s3.download_file(bucket_name, object_key, file_path)
            print(f"File '{object_key}' downloaded successfully.")

