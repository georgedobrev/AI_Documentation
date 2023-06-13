import json

from Model.t5_model import local_llm


def callbackMessage(ch, method, properties, body):
        data = json.loads(body.decode())
        command_name = data['CommandName']
        input_question = data['Payload']['InputQuestion']
        print(f'Hello Admin, we received your message => {data} commandName => {command_name} input_question => {input_question}')

        answer = local_llm(input_question)
        print(answer)
        response_data = {
                'CommandName': command_name,
                'Payload': {
                        'InputQuestion': input_question,
                        'Answer': answer
                }
        }
        response_body = json.dumps(response_data)
        routing_key = 'DocuAurora-api/RabittMQMessage'  # Specify the routing key for the response queue

        ch.basic_publish(exchange='', routing_key=routing_key, body=response_body)
        print('Published answer:', response_body)

