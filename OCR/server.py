import requests
from PIL import Image
import pytesseract
from io import BytesIO
from flask import Flask, request
import logging
import sys

app = Flask(__name__)

@app.route('/process_image', methods=['POST'])
def process_image():
    image_url = request.form.get('image_url')
    
    # Download the image
    response = requests.get(image_url)
    image = Image.open(BytesIO(response.content))
    
    # Perform OCR on the image
    text = pytesseract.image_to_string(image)
    
    return text

if __name__ == '__main__':
    app.logger.addHandler(logging.StreamHandler(sys.stdout))
    app.logger.setLevel(logging.INFO)
    app.run(port=5001)