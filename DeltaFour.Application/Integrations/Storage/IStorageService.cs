namespace DeltaFour.Application.Integrations.Storage
{
    /// <summary>
    /// Abstração para operações de armazenamento de arquivos (upload, remoção e listagem).
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Realiza o upload de um arquivo para o armazenamento configurado.
        /// </summary>
        /// <param name="arquivo">Stream contendo o conteúdo do arquivo.</param>
        /// <param name="nomeArquivo">Caminho/nome que o arquivo terá no armazenamento.</param>
        /// <param name="contentType">Tipo MIME do arquivo.</param>
        /// <returns>O caminho/nome do arquivo armazenado.</returns>
        Task<string> SalvarArquivo(Stream arquivo, string nomeArquivo, string contentType);

        /// <summary>
        /// Remove um arquivo específico do armazenamento.
        /// </summary>
        Task RemoverArquivo(string caminhoArquivo);

        /// <summary>
        /// Lista as URLs públicas dos arquivos existentes em uma pasta.
        /// </summary>
        Task<List<string>> ListarArquivos(string pastaArquivos);

        /// <summary>
        /// Retorna a URL base pública do armazenamento configurado.
        /// </summary>
        string GetBaseUrl();
    }
}
