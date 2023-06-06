from flask import request
from flask_restful import Resource

class TextProcessor(Resource):
    def get(self):
        return {'message': 'Hello team'}

    def post(self):
        data = request.get_json()
        text = data['hello team']
        print(text)
        return {'message': 'Text processed successfully'}, 200