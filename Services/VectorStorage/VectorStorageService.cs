namespace RAG_Code_Base.Services.VectorStorage
{
    public class VectorStorageService
    {
        private readonly ILogger<VectorStorageService> _logger;
        // Здесь будет сервис Максима, когда он его создаст

        public VectorStorageService(ILogger<VectorStorageService> logger)
        {
            _logger = logger;
        }

        public async Task SaveEmbeddingAsync(Guid blockId, float[] embedding)
        {
            _logger.LogInformation("Сохранение эмбеддинга для блока {BlockId}", blockId);
            // TODO: вызов метода сервиса Максима
        }
    }
}
