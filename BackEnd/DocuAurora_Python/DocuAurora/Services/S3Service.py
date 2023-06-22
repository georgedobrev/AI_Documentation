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
            file_path = os.path.join('..\Model', object_key)
            os.makedirs(os.path.dirname(file_path), exist_ok=True)

            print(file_path)
            self.s3.download_file(bucket_name, object_key, file_path)
            print(f"File '{object_key}' downloaded successfully.")

    # def send_file(self, file_path):
    #     try:
    #         print("File sent successfully.")
    #     except subprocess.CalledProcessError as e:
    #         print(f"Error sending file: {e}")

    # def process_files(self, bucket_name, object_keys):
    #     for object_key in object_keys:
    #         file_name = os.path.basename(object_key)
    #         file_path = f"LOCAL_DIRECTORY/{file_name}"
    #         self.download_file(bucket_name, object_key, file_path)
    #         self.send_file(file_path)
    #         os.remove(file_path)
