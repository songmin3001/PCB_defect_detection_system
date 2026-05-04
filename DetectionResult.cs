// Models/DetectionResult.cs
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class BBox
{
    [JsonPropertyName("x1")] public int X1 { get; set; }
    [JsonPropertyName("y1")] public int Y1 { get; set; }
    [JsonPropertyName("x2")] public int X2 { get; set; }
    [JsonPropertyName("y2")] public int Y2 { get; set; }
}

public class Detection
{
    [JsonPropertyName("class_id")] public int ClassId { get; set; }
    [JsonPropertyName("class_name")] public string ClassName { get; set; }
    [JsonPropertyName("confidence")] public double Confidence { get; set; }
    [JsonPropertyName("bbox")] public BBox BBox { get; set; }
}

public class PredictResponse
{
    [JsonPropertyName("defect_found")] public bool DefectFound { get; set; }
    [JsonPropertyName("count")] public int Count { get; set; }
    [JsonPropertyName("detections")] public List<Detection> Detections { get; set; }
}
