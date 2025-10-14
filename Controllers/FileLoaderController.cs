using Microsoft.AspNetCore.Mvc;
using RAG_Code_Base.Services.DataLoader;

namespace RAG_Code_Base.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileLoaderController : ControllerBase
    {
        private readonly FileLoaderService _fileLoaderService;

        public FileLoaderController(FileLoaderService fileLoaderService)
        {
            _fileLoaderService = fileLoaderService;
        }

        [HttpPost("upload")]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран.");

            var savedFile = _fileLoaderService.SaveFile(file);
            return Ok(savedFile);
        }
    }
}
