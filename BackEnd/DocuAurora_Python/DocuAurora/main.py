
from flask import Flask
from Services.RabbitMQService import CreateConnectionRabbitMQ

app = Flask(__name__)



if __name__ == '__main__':
    host_name = 'localhost'
    exchange_name = 'DocuAurora-Exchange'
    queue_name = 'DocuAurora-Queue'
    message_routing_key = 'DocuAurora-api/RabittMQMessage'
    file_routing_key = 'DocuAurora-api/RabittMQFile'

    rabbitmq_service = CreateConnectionRabbitMQ(host_name, exchange_name, queue_name, message_routing_key, file_routing_key)
    rabbitmq_service.connect()
    app.run(host='0.0.0.0', port=5000, debug=True)