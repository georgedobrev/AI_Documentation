from flask import Flask, request, jsonify
import PyPDF2
import re

app = Flask(__name__)
app.config['MAX_CONTENT_LENGTH'] = 100 * 1024 * 1024
@app.route('/api/extract_text', methods=['POST'])
def extract_text():
    file = request.files['pdf_file']

    reader = PyPDF2.PdfReader(file.stream)
    text = ''
    for page in reader.pages:
        text += page.extract_text()

    text_array = re.split('[.;!?]', text)

    return jsonify(text_array)

if __name__ == '__main__':
    app.run(port=5000, debug=True)