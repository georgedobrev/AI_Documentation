import pika
import configparser
import json

class RabbitMQService:
    def __init__(self,config_file):
        self.config_file = config_file
        self.host = self._get_config_value('RabbitMQ', 'host_name')
        self.username = self._get_config_value('RabbitMQ', 'username')
        self.password = self._get_config_value('RabbitMQ', 'password')
        self.connection = None
        self.channel = None
        self.exchange_name = self._get_config_value('RabbitMQ', 'exchange_name')
        self.message_queue_name =self._get_config_value('RabbitMQ', 'message_queue_name')
        self.file_queue_name =self._get_config_value('RabbitMQ', 'file_queue_name')
        self.message_routing_key = self._get_config_value('RabbitMQ', 'message_routing_key')
        self.file_routing_key =self._get_config_value('RabbitMQ', 'file_routing_key')

    def _get_config_value(self, section, key):
        config = configparser.ConfigParser()
        config.read(self.config_file)
        return config.get(section, key)

    def connect(self):
        credentials = pika.PlainCredentials(self.username, self.password)
        parameters = pika.ConnectionParameters(self.host, credentials=credentials)
        self.connection = pika.BlockingConnection(parameters)
        self.channel = self.connection.channel()

    def create_queues(self):
        self.channel.exchange_declare(exchange=self.exchange_name, exchange_type='direct')
        self.channel.queue_declare(queue=self.message_queue_name)
        self.channel.queue_declare(queue=self.file_queue_name)
        self.channel.queue_bind(exchange=self.exchange_name, queue=self.message_queue_name, routing_key=self.message_routing_key)
        self.channel.queue_bind(exchange=self.exchange_name, queue=self.file_queue_name, routing_key=self.file_routing_key)

    def consume_messages(self, queue_callbacks):
        for queue_name, callback in queue_callbacks.items():
            self.channel.basic_consume(queue=queue_name, on_message_callback=callback, auto_ack=True)
        self.channel.start_consuming()

    def close_connection(self):
        if self.connection and not self.connection.is_closed:
            self.connection.close()


