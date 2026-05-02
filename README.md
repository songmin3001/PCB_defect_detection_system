# PCB_defect_detection_system

YOLOv8 기반 PCB 불량 자동 검출 시스템입니다. 
Python FastAPI 서버에서 학습된 모델이 추론을 수행하고, C# WPF 클라이언트가 결과를 실시간으로 시각화합니다.

---

## 시스템 구조

```
PCBInspector/
├── server/                  # Python FastAPI 서버
│   ├── main.py
│   └── best.pt              # 학습된 YOLOv8 모델
└── client/                  # C# WPF 클라이언트
    ├── MainWindow.xaml
    ├── MainWindow.xaml.cs
    ├── Models/
    │   └── DetectionResult.cs
    └── Services/
        └── ApiService.cs
```

---

## 기술 스택

| 구분 | 기술 |
|------|------|
| 모델 | YOLOv8 (Ultralytics) |
| 백엔드 | Python 3.10+, FastAPI, OpenCV |
| 프론트엔드 | C# .NET 6+, WPF |
| 통신 | REST API (HTTP multipart/form-data) |

---

## 설치 및 실행

### Python 서버

```bash
cd server
pip install fastapi uvicorn ultralytics opencv-python
uvicorn main:app --host 0.0.0.0 --port 8000
```

### C# 클라이언트

Visual Studio에서 `PCBInspector.sln` 열고 빌드 후 실행.  
별도 NuGet 패키지 설치 불필요 (`System.Net.Http`, `System.Text.Json` 기본 포함).

---

## 사용 방법

1. Python 서버 먼저 실행
2. WPF 앱 실행
3. **이미지 불러오기** 버튼으로 PCB 이미지 선택
4. **검사 시작** 버튼 클릭
5. 화면에 불량 유형, 신뢰도, 바운딩박스 실시간 출력

---

## 검출 불량 유형

| ID | 유형 |
|----|------|
| 0 | 단락 (Short) |
| 1 | 개방 (Open) |
| 2 | 브리지 (Bridge) |
| 3 | 미납땜 (Missing Solder) |
| 4 | 과납 (Excess Solder) |
| 5 | 패턴손상 (Pattern Damage) |

---

## API 명세

### `POST /predict`

| 항목 | 내용 |
|------|------|
| Content-Type | multipart/form-data |
| 파라미터 | `file` (이미지 파일) |

**응답 예시**

```json
{
  "defect_found": true,
  "count": 2,
  "detections": [
    {
      "class_id": 0,
      "class_name": "단락",
      "confidence": 0.923,
      "bbox": { "x1": 120, "y1": 85, "x2": 210, "y2": 160 }
    }
  ]
}
```

---

## 성과

- 데이터셋: 불량 유형별 직접 재현 촬영, 총 800장 (다양한 각도·조명 조건 적용)
- 모델 성능: **mAP@0.5 = 0.963**
- 담당 역할: 데이터셋 구축 / C# WPF UI 및 API 연동 개발
