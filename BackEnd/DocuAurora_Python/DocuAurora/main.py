import configparser
from pathlib import Path

from flask import Flask

from Services.RabbitMQService import RabbitMQService

from Services.RabbitMQFileConsume import callbackFile
from Services.RabbitMQMessageConsume import callbackMessage

from Model.t5_model import setup_model, load_documents, setup_pinecone, setup_retrieval_qa, split_text, ask_question

# from Model.t5_model import local_llm

app = Flask(__name__)

if __name__ == '__main__':
    script_dir = Path(__file__).resolve().parent

    config_file = script_dir / "config.ini"

    rabbitmq_service = RabbitMQService(config_file)
    rabbitmq_service.connect()
    #to optimize this part!
    queue1_name = 'DocuAurora-Message-Queue'
    queue2_name = 'DocuAurora-File-Queue'

    #queue/callback container
    queue_callbacks = {
        queue2_name: callbackFile,
        queue1_name: callbackMessage
    }

    rabbitmq_service.create_queues()
    rabbitmq_service.consume_messages(queue_callbacks)

    # local_llm = setup_model('google/flan-t5-base')
    # documents = load_documents('C:/C# - learning/AI_Documentation/BackEnd/DocuAurora_Python/DocuAurora/Model/Bulgaria.pdf')
    # documents = load_documents('C:/C# - learning/AI_Documentation/BackEnd/DocuAurora_Python/DocuAurora/Model/Test2.pdf')
    # chunks = split_text(documents)
    # retriever = setup_pinecone(chunks, "hkunlp/instructor-xl", 'b4b7947c-96fd-4c95-9785-9c8ced03b64b', 'us-west1-gcp-free', "test2")
    # qa_chain = setup_retrieval_qa(local_llm, retriever)
    #
    # # Asking Questions
    # ask_question(qa_chain, "who have contribution for book history in Bulgaria?")
    # ask_question(qa_chain, "who is John Draper?")
    # ask_question(qa_chain, "what is the capital of Germany?")
    
    app.run(host='0.0.0.0', port=5000, debug=True)


