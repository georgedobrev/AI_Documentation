import pika
import configparser
import json

class RabbitMQService:
    def __init__(self, config_file):
        self.config_file = config_file
        self.host_name ='host_name'
        self.exchange_name = 'exchange_name'

        # to do change config file
        self.message_queue_name = "DocuAurora-MessageT-Queue"
        self.file_queue_name = "DocuAurora-FileT-Queue"
        self.message_routing_key ='message_routing_key'
        self.file_routing_key = 'RabbitMQ', 'file_routing_key'
        self.connection = None
        self.channel = None

    def _get_config_value(self, section, key):
        config = configparser.ConfigParser()
        config.read(self.config_file)
        return config.get(section, key)

    def connect(self):
        parameters = pika.URLParameters('amqp://guest:guest@localhost:5672/%2F')
        self.connection = pika.SelectConnection(parameters=parameters, on_open_callback=self.on_connection_open)


    def on_connection_open(self, connection):
        self.channel = self.connection.channel()
        self.channel.exchange_declare(exchange=self.exchange_name, exchange_type='direct')
        self.channel.queue_declare(queue=self.message_queue_name)
        self.channel.queue_declare(queue=self.file_queue_name)
        self.channel.queue_bind(exchange=self.exchange_name, queue=self.message_queue_name,
                                routing_key=self.file_routing_key)
        self.channel.queue_bind(exchange=self.exchange_name, queue=self.file_queue_name,
                                routing_key=self.message_routing_key)

    def start_consuming(self, queue_name, callback):

        self.channel.basic_consume(
            queue=queue_name,
            on_message_callback=callback,
            auto_ack=True
        )

        print(f' [*] {queue_name} waiting for messages. To exit, press CTRL+C')

        self.channel.start_consuming()
