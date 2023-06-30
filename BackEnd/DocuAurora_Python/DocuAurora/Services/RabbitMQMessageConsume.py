import json
import os
from pathlib import Path
from Services.AnswerGeneratorService import AnswerGeneratorService


def callbackMessage(ch, method, properties, body):
    data = json.loads(body.decode())
    command_name = data['CommandName']
    input_question = data['Payload']['InputQuestion']
    print(
        f'Hello Admin, we received your message => {data} commandName => {command_name} input_question => {input_question}')

    main_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

    config_file_path = os.path.join(main_dir, 'config.ini')

    answer_generator = AnswerGeneratorService(config_file_path)
    answer_generator.load_existing_index(command_name)
    answer = answer_generator.generate_answer(input_question)

    print(answer)
    response_data = {
        'CommandName': command_name,
        'Payload': {
            'InputQuestion': input_question,
            'Answer': answer
        }
    }
    response_body = json.dumps(response_data)
    ch.basic_publish('', routing_key=properties.reply_to, body=response_body)
    print('Published answer:', response_body)
