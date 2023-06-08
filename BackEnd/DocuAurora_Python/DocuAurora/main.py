from flask import Flask
from flask_restful import Api
from flask import Flask, request, jsonify
from Model.t5_model import generate_text

app = Flask(__name__)

@app.route("/predict", methods=["POST"])
def predict():
    data = request.get_json()['text']
    prediction = generate_text(data)
    return jsonify({'prediction': prediction})

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)