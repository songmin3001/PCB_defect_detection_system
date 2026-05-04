// Services/ApiService.cs
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

public class ApiService
{
    private readonly HttpClient _client = new HttpClient();
    private const string ApiUrl = "http://localhost:8000/predict";

    public async Task<PredictResponse> PredictAsync(string imagePath)
    {
        using var form = new MultipartFormDataContent();
        var fileBytes = File.ReadAllBytes(imagePath);
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = 
            new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        form.Add(fileContent, "file", Path.GetFileName(imagePath));

        var response = await _client.PostAsync(ApiUrl, form);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PredictResponse>(json);
    }
}
