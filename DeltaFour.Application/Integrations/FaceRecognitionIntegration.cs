using System.Text.Json;

namespace DeltaFour.Application.Integrations;

public class FaceRecognitionIntegration : IFaceRecognitionIntegration
{
    private readonly HttpClient _httpClient;

    //TODO: Realizar testes para achar a porcentagem ideal 
    private const int SimilarityPercentage = 98;

    public FaceRecognitionIntegration(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private byte[] ConvertBase64ToBytes(string base64)
    {
        if (base64.Contains(","))
            base64 = base64.Split(',')[1];

        return Convert.FromBase64String(base64);
    }

    public async Task<bool> ChecksIfFaceMatchs(string image, string embeddings)
    {
        using var form = new MultipartFormDataContent();

        var imageBytes = ConvertBase64ToBytes(image);
        var imageContent = new ByteArrayContent(imageBytes);

        imageContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

        form.Add(imageContent, "image", "face.jpg");
        form.Add(new StringContent(embeddings), "embedding");

        var response = await _httpClient.PostAsync("/compare", form);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var similarityString = doc.RootElement
            .GetProperty("similaridade")
            .GetString()!;

        var similarity = double.Parse(similarityString.Replace("%", ""));

        return similarity >= SimilarityPercentage;
    }

    public async Task<string> GetFaceEmbeddings(string image)
    {
        var imageBytes = ConvertBase64ToBytes(image);

        using var form = new MultipartFormDataContent();

        var imageContent = new ByteArrayContent(imageBytes);

        imageContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

        form.Add(imageContent, "image", "face.jpg");

        var response = await _httpClient.PostAsync("/embedding", form);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        return doc.RootElement.GetProperty("embedding").GetString()!;
    }
}
