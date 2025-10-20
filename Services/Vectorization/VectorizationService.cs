using LMKit.Model;
using LMKit.Embeddings;
using RAG_Code_Base.Models;

namespace RAG_Code_Base.Services.Vectorization
{
    public class VectorizationService
    {
        private readonly LM _model;
        private readonly Embedder _embedder;

        public VectorizationService()
        {
            var modelPath = Path.Combine(Directory.GetCurrentDirectory(), "LM", "bge-m3-Q4_K_M.gguf");
            _model = new LM(modelPath);
            _embedder = new Embedder(_model);
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var embedding = await _embedder.GetEmbeddingsAsync(text, CancellationToken.None);
            return embedding;
        }
    }
}
