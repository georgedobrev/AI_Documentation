
from flask import Flask
from Services.RabbitMQService import RabbitMQService
from config import host_name, exchange_name, queue_name, message_routing_key, file_routing_key

app = Flask(__name__)



if __name__ == '__main__':


    rabbitmq_service = RabbitMQService(host_name, exchange_name, queue_name, message_routing_key, file_routing_key)
    rabbitmq_service.connect()
    rabbitmq_service.start_consuming()

    app.run(host='0.0.0.0', port=5000, debug=True)