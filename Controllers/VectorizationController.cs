using Microsoft.AspNetCore.Mvc;
using RAG_Code_Base.Services.Vectorization;

namespace RAG_Code_Base.Controllers
{
    [ApiController]
    [Route("api/vectorization")]
    public class VectorizationController: ControllerBase
    {
        private readonly VectorizationService _vectorizationService;

        public VectorizationController(VectorizationService vectorizationService)
        {
            _vectorizationService = vectorizationService;
        }

        [HttpPost("test")]
        public async Task<IActionResult> TaskEmbedding([FromBody] string text)
        {
            var embedding = await _vectorizationService.GenerateEmbeddingAsync(text);
            return Ok(new
            {
                text,
                embeddingLength = embedding.Length,
                firstFewValues = embedding.Take(5)
            });
        }
    }
}
