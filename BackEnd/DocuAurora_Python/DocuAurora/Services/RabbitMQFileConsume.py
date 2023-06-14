import json

def callbackFile(ch, method, properties, body, publish_message):
        # data = json.loads(body.decode())
        # command_name = data['BucketName']
        # file_key = data['DocumentNames']
        # print(f'Hello Admin, we received your message => {data} commandName => {command_name} file_key => {file_key}')
         print(f'file queue {body}')


         publish_message("primerno")

        #to do return messages. how to write public message