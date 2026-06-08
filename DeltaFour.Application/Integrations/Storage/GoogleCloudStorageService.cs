using Google.Cloud.Storage.V1;

namespace DeltaFour.Application.Integrations.Storage
{
    /// <summary>
    /// Implementação do serviço de armazenamento utilizando o Google Cloud Storage.
    /// Responsável por realizar operações de upload, remoção e listagem de arquivos
    /// dentro de um bucket configurado.
    /// </summary>
    public class GoogleCloudStorageService : IStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly string _storageBaseUrl;

        /// <summary>
        /// Construtor que inicializa o cliente do Google Cloud Storage
        /// e carrega as configurações a partir das variáveis de ambiente.
        /// As credenciais são resolvidas via Application Default Credentials
        /// (variável de ambiente GOOGLE_APPLICATION_CREDENTIALS).
        /// </summary>
        public GoogleCloudStorageService()
        {
            _storageClient = StorageClient.Create();
            _bucketName = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_BUCKET_NAME")!;
            _storageBaseUrl = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_STORAGE_BASE_URL")!;
        }

        /// <summary>
        /// Realiza o upload de um arquivo para o bucket configurado.
        /// </summary>
        /// <param name="arquivo">Stream contendo o arquivo a ser enviado.</param>
        /// <param name="nomeArquivo">Caminho/nome que o arquivo terá dentro do bucket.</param>
        /// <param name="contentType">Tipo MIME do arquivo.</param>
        /// <returns>Retorna o caminho do arquivo armazenado.</returns>
        public async Task<string> SalvarArquivo(Stream arquivo, string nomeArquivo, string contentType)
        {
            await _storageClient.UploadObjectAsync(
                _bucketName,
                nomeArquivo,
                contentType,
                arquivo);

            return nomeArquivo;
        }

        /// <summary>
        /// Remove um arquivo específico do bucket.
        /// </summary>
        /// <param name="caminhoArquivo">Caminho do arquivo dentro do bucket.</param>
        public async Task RemoverArquivo(string caminhoArquivo)
        {
            await _storageClient.DeleteObjectAsync(_bucketName, caminhoArquivo);
        }

        /// <summary>
        /// Lista todos os arquivos existentes dentro de uma pasta do bucket.
        /// </summary>
        /// <param name="pastaArquivos">Prefixo/pasta onde os arquivos estão armazenados.</param>
        /// <returns>Lista contendo as URLs completas dos arquivos.</returns>
        public Task<List<string>> ListarArquivos(string pastaArquivos)
        {
            var objetos = _storageClient.ListObjects(_bucketName, pastaArquivos);

            var urls = objetos
                // Ignora "pastas" virtuais criadas no bucket
                .Where(o => !o.Name.EndsWith("/"))
                // Monta a URL pública do arquivo
                .Select(o => $"{_storageBaseUrl}/{_bucketName}/{o.Name}")
                .ToList();

            return Task.FromResult(urls);
        }

        /// <summary>
        /// Retorna a URL base do bucket configurado.
        /// Utilizada para montar URLs públicas dos arquivos armazenados.
        /// </summary>
        public string GetBaseUrl()
        {
            return $"{_storageBaseUrl}/{_bucketName}";
        }
    }
}
