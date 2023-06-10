import pika
import configparser
import json

class RabbitMQService:
    def __init__(self, config_file):
        self.config_file = config_file
        self.host_name = self._get_config_value('RabbitMQ', 'host_name')
        self.exchange_name = self._get_config_value('RabbitMQ', 'exchange_name')
        self.queue_name = self._get_config_value('RabbitMQ', 'queue_name')
        self.message_routing_key = self._get_config_value('RabbitMQ', 'message_routing_key')
        self.file_routing_key = self._get_config_value('RabbitMQ', 'file_routing_key')
        self.connection = None
        self.channel = None

    def _get_config_value(self, section, key):
        config = configparser.ConfigParser()
        config.read(self.config_file)
        return config.get(section, key)

    def connect(self):
        self.connection = pika.BlockingConnection(pika.ConnectionParameters(host=self.host_name))
        self.channel = self.connection.channel()
        self.channel.exchange_declare(exchange=self.exchange_name, exchange_type='direct')
        self.channel.queue_declare(queue=self.queue_name)
        self.channel.queue_bind(exchange=self.exchange_name, queue=self.queue_name, routing_key=self.file_routing_key)
        self.channel.queue_bind(exchange=self.exchange_name, queue=self.queue_name, routing_key=self.message_routing_key)

    def start_consuming(self):
        def callback(ch, method, properties, body):
            routing_key = method.routing_key
            if routing_key == self.file_routing_key:
                data = json.loads(body.decode())
                command_name = data['BucketName']
                file_key = data['DocumentNames']
                print(
                    f'Hello Admin, we received your message => {data} commandName => {command_name} file_key => {file_key}')
            elif routing_key == self.message_routing_key:
                data = json.loads(body.decode())
                command_name = data['CommandName']
                input_question = data['Payload']['InputQuestion']
                print(
                    f'Hello Admin, we received your message => {data} commandName => {command_name} input_question => {input_question}')

        self.channel.basic_consume(
            queue=self.queue_name,
            on_message_callback=callback,
            auto_ack=True
        )

        print(' [*] Waiting for messages. To exit, press CTRL+C')

        self.channel.start_consuming()