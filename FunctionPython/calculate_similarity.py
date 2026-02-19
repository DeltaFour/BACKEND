import numpy as np

def calculate_similarity(first_embeddding, second_embeddding):
    emb1 = np.array(first_embeddding)
    emb2 = np.array(second_embeddding)

    cosine_similarity = np.dot(emb1, emb2) / (np.linalg.norm(emb1) * np.linalg.norm(emb2))
    percent = ((cosine_similarity + 1) / 2) * 100
    percent = round(float(percent), 2)

    return percent