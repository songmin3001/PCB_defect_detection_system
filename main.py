# main.py
from fastapi import FastAPI, File, UploadFile
from fastapi.responses import JSONResponse
from ultralytics import YOLO
import numpy as np
import cv2

app = FastAPI()
model = YOLO("best.pt")  # 학습된 모델 경로

DEFECT_CLASSES = ["단락", "개방", "브리지", "미납땜", "과납", "패턴손상"]  # 실제 클래스명으로 수정

@app.post("/predict")
async def predict(file: UploadFile = File(...)):
    contents = await file.read()
    nparr = np.frombuffer(contents, np.uint8)
    img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

    results = model(img)[0]
    detections = []

    for box in results.boxes:
        cls_id = int(box.cls[0])
        conf = float(box.conf[0])
        x1, y1, x2, y2 = map(int, box.xyxy[0])
        detections.append({
            "class_id": cls_id,
            "class_name": DEFECT_CLASSES[cls_id],
            "confidence": round(conf, 3),
            "bbox": {"x1": x1, "y1": y1, "x2": x2, "y2": y2}
        })

    return JSONResponse(content={
        "defect_found": len(detections) > 0,
        "count": len(detections),
        "detections": detections
    })