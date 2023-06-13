import json
def callbackMessage(ch, method, properties, body):
        # data = json.loads(body.decode())
        # command_name = data['CommandName']
        # input_question = data['Payload']['InputQuestion']
        # print(f'Hello Admin, we received your message => {data} commandName => {command_name} input_question => {input_question}')
        print(f'message queue {body}')