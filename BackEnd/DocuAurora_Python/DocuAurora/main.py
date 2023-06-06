from flask import Flask
from flask_restful import Api
from Resources.text_processor import TextProcessor

app = Flask(__name__)
api = Api(app)

api.add_resource(TextProcessor, '/')

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)