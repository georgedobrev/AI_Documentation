import pika
class CreateConnectionRabbitMQ:
    def __init__(self, host_name, exchange_name, queue_name, message_routing_key, file_routing_key):
        # Initialize any dependencies or resources required by the service
        self.host_name = host_name
        self.exchange_name = exchange_name
        self.queue_name = queue_name
        self.message_routing_key = message_routing_key
        self.file_routing_key = file_routing_key
        pass
    def connect(self):
         connection = pika.BlockingConnection(pika.ConnectionParameters(host=self.host_name))
         channel = connection.channel()
         channel.exchange_declare(exchange=self.exchange_name, exchange_type='direct')
         channel.queue_declare(queue=self.queue_name)
         channel.queue_bind(exchange=self.exchange_name, queue=self.queue_name, routing_key=self.file_routing_key)
         channel.queue_bind(exchange=self.exchange_name, queue=self.queue_name, routing_key=self.message_routing_key)
