import json
from pathlib import Path
from AnswerGeneratorService import AnswerGeneratorService


# from Model.t5_model import local_llm


def callbackMessage(ch, method, properties, body):
    data = json.loads(body.decode())
    command_name = data['CommandName']
    input_question = data['Payload']['InputQuestion']
    print(
        f'Hello Admin, we received your message => {data} commandName => {command_name} input_question => {input_question}')

    script_dir = Path(__file__).resolve().parent
    config_file = script_dir / "config.ini"

    answer_generator = AnswerGeneratorService(config_file)
    question = "Who have contribution for book history in Bulgaria?"
    answer = answer_generator.generate_answer(question)

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
