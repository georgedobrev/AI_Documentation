
from flask import Flask
from Services.RabbitMQService import CreateConnectionRabbitMQ
from config import host_name, exchange_name, queue_name, message_routing_key, file_routing_key

app = Flask(__name__)



if __name__ == '__main__':


    rabbitmq_service = CreateConnectionRabbitMQ(host_name, exchange_name, queue_name, message_routing_key, file_routing_key)
    rabbitmq_service.connect()
    app.run(host='0.0.0.0', port=5000, debug=True)