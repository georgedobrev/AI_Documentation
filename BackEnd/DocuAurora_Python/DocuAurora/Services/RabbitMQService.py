import pika
import configparser
import json

class RabbitMQService:
    def __init__(self, host='localhost', username='guest', password='guest'):
        self.host = host
        self.username = username
        self.password = password
        self.connection = None
        self.channel = None

    def connect(self):
        credentials = pika.PlainCredentials(self.username, self.password)
        parameters = pika.ConnectionParameters(self.host, credentials=credentials)
        self.connection = pika.BlockingConnection(parameters)
        self.channel = self.connection.channel()

    def create_queues(self, queue1_name, queue2_name, exchange_name, routing_key1, routing_key2):
        self.channel.exchange_declare(exchange=exchange_name, exchange_type='direct')
        self.channel.queue_declare(queue=queue1_name)
        self.channel.queue_declare(queue=queue2_name)
        self.channel.queue_bind(exchange=exchange_name, queue=queue1_name, routing_key=routing_key1)
        self.channel.queue_bind(exchange=exchange_name, queue=queue2_name, routing_key=routing_key2)

    def consume_messages(self, queue_callbacks):
        for queue_name, callback in queue_callbacks.items():
            self.channel.basic_consume(queue=queue_name, on_message_callback=callback, auto_ack=True)
        self.channel.start_consuming()

    def close_connection(self):
        if self.connection and not self.connection.is_closed:
            self.connection.close()


