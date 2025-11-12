using Microsoft.AspNetCore.Mvc;
using RAG_Code_Base.Services.Explanation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RAG_Code_Base.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExplanationController : ControllerBase
    {
        private readonly ExplanationService _explanationService;

        public ExplanationController()
        {
            _explanationService = new ExplanationService();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ExplanationRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("Вопрос не может быть пустым.");

            string answer = await _explanationService.ExplainAsync(request.Question, request.Contexts, cancellationToken);

            return Ok(new { Answer = answer });
        }
    }

    public class ExplanationRequest
    {
        public string Question { get; set; } = string.Empty;
        public List<string>? Contexts { get; set; }
    }
}
