import pika


class RabbitMQService:
    def __init__(self, host_name, exchange_name, queue_name, message_routing_key, file_routing_key):
        # Initialize any dependencies or resources required by the service
        self.host_name = host_name
        self.exchange_name = exchange_name
        self.queue_name = queue_name
        self.message_routing_key = message_routing_key
        self.file_routing_key = file_routing_key
        self.connection = None
        self.channel = None
        pass

    def connect(self):
        self.connection = pika.BlockingConnection(pika.ConnectionParameters(host=self.host_name))
        self.channel = self.connection.channel()
        self.channel.exchange_declare(exchange=self.exchange_name, exchange_type='direct')
        self.channel.queue_declare(queue=self.queue_name)
        self.channel.queue_bind(exchange=self.exchange_name, queue=self.queue_name, routing_key=self.file_routing_key)
        self.channel.queue_bind(exchange=self.exchange_name, queue=self.queue_name, routing_key=self.message_routing_key)

    def start_consuming(self):
        def callback(ch, method, properties, body):
            print(" [x] Received %r" % body)


        self.channel.basic_consume(
            queue=self.queue_name,
            on_message_callback=callback,
            auto_ack=True
        )

        print(' [*] Waiting for messages. To exit, press CTRL+C')

        self.channel.start_consuming()
