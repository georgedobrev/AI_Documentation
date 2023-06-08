import requests

# Replace 'http://localhost:5000' with the URL of your Flask application
response = requests.post('http://localhost:5000/predict', json={"text": "A step by step recipe to make bolognese pasta:"})

print(response.json())