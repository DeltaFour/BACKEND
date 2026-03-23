namespace DeltaFour.Application.Integrations;

public interface IFaceRecognitionIntegration
{
    public Task<string> GetFaceEmbeddings(string image);
    public Task<bool> ChecksIfFaceMatchs(string image, string embeddings);
}
