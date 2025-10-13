using RAG_Code_Base.Models;

namespace RAG_Code_Base.Services.DataLoader
{
    public class FileLoaderService
    {
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        private readonly FileTypeResolver _typeResolver = new(); 
        public FileLoaderService()
        {
            if (!Directory.Exists(_storagePath))
                Directory.CreateDirectory(_storagePath);
        }

        public FileItem SaveFile(IFormFile file)
        {
            var filePath = Path.Combine(_storagePath, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var fileItem = new FileItem
            {
                FileName = file.FileName,
                FilePath = filePath,
                FileType = _typeResolver.GetFileType(file.FileName),
            };

            return fileItem;
        }
    }
}
