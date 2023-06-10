import os
import boto3
import subprocess

class S3Service:
    def __init__(self, region_name=None):
        aws_access_key_id = os.environ.get('AWS_ACCESS_KEY_ID')
        aws_secret_access_key = os.environ.get('AWS_SECRET_ACCESS_KEY')

        self.s3 = boto3.client('s3', aws_access_key_id=aws_access_key_id, aws_secret_access_key=aws_secret_access_key, region_name=region_name)

    def download_file(self, bucket_name, object_key, file_path):
        self.s3.download_file(bucket_name, object_key, file_path)
        print("File downloaded successfully.")

    def send_file(self, file_path, program_path):
        try:
            subprocess.run([program_path, file_path], check=True)
            print("File sent successfully.")
        except subprocess.CalledProcessError as e:
            print(f"Error sending file: {e}")

    def process_files(self, bucket_name, object_keys, program_path):
        for object_key in object_keys:
            file_name = os.path.basename(object_key)
            file_path = f"LOCAL_DIRECTORY/{file_name}"
            self.download_file(bucket_name, object_key, file_path)
            self.send_file(file_path, program_path)
            os.remove(file_path)
