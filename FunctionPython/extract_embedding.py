import face_recognition
import numpy as np
from fastapi import FastAPI, UploadFile, File, Form
from io import BytesIO
from PIL import Image

def extract_embedding(file_byte):
    # file_obj.seek(0)
    # image_data = file_obj.read()

    img = Image.open(BytesIO(file_byte)).convert("RGB")
    img = np.array(img).astype("uint8")

    encodings = face_recognition.face_encodings(img)
    if len(encodings) == 0:
        return None

    return encodings[0].tolist()

def calculate_similarity(first_embeddding, second_embeddding):
    emb1 = np.array(first_embeddding)
    emb2 = np.array(second_embeddding)

    cosine_similarity = np.dot(emb1, emb2) / (np.linalg.norm(emb1) * np.linalg.norm(emb2))
    percent = ((cosine_similarity + 1) / 2) * 100
    percent = round(float(percent), 2)

    return percent