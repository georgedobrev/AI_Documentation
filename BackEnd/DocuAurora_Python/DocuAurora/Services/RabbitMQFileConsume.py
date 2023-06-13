import json

def callbackFile(ch, method, properties, body):
        data = json.loads(body.decode())
        command_name = data['BucketName']
        file_key = data['DocumentNames']
        print(f'Hello Admin, we received your message => {data} commandName => {command_name} file_key => {file_key}')
