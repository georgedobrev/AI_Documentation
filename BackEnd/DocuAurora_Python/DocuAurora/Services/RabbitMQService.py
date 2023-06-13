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

    def create_queues(self, queue1_name, queue2_name):
        self.channel.queue_declare(queue=queue1_name)
        self.channel.queue_declare(queue=queue2_name)

    def consume_messages(self, queue_name, callback):
        self.channel.basic_consume(queue=queue_name, on_message_callback=callback, auto_ack=True)
        self.channel.start_consuming()

    def close_connection(self):
        if self.connection and not self.connection.is_closed:
            self.connection.close()

    # Message callback function for queue 1


def callback_queue1(channel, method, properties, body):
    print(f"Received message from Queue 1: {body.decode()}")

    # Message callback function for queue 2


def callback_queue2(channel, method, properties, body):
    print(f"Received message from Queue 2: {body.decode()}")
