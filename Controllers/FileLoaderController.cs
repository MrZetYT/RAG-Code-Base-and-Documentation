using Microsoft.AspNetCore.Mvc;
using RAG_Code_Base.Services.DataLoader;

namespace RAG_Code_Base.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileLoaderController : ControllerBase
    {
        private readonly FileLoaderService _fileLoaderService;
        private readonly FileValidator _fileValidator;

        public FileLoaderController(FileLoaderService fileLoaderService, FileValidator fileValidator)
        {
            _fileLoaderService = fileLoaderService;
            _fileValidator = fileValidator;
        }

        [HttpPost("upload")]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран.");

            var validationResult = _fileValidator.Validate(file);
            if (validationResult.IsValid == false)
            {
                return BadRequest(validationResult.ErrorMessage);
            }

            var savedFile = _fileLoaderService.SaveFile(file);
            return Ok(savedFile);
        }

        [HttpGet("")]
        public IActionResult GetFilesList()
        {
            var gotFiles = _fileLoaderService.GetAllFiles();
            return Ok(gotFiles);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteFile(Guid id)
        {
            var deletedFile = _fileLoaderService.DeleteFile(id);
            if (!deletedFile) return NotFound("Файл не найден.");
            return NoContent();
        }
    }
}
