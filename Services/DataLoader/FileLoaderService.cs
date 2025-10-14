using RAG_Code_Base.Database;
using RAG_Code_Base.Models;

namespace RAG_Code_Base.Services.DataLoader
{
    public class FileLoaderService
    {
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        private readonly FileTypeResolver _typeResolver = new();
        private readonly ApplicationDbContext _applicationDbContext;
        public FileLoaderService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
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

            _applicationDbContext.FileItems.Add(fileItem);
            _applicationDbContext.SaveChanges();

            return fileItem;
        }

        public List<FileItem> GetAllFiles()
        {
            return _applicationDbContext.FileItems.ToList();
        }
    }
}
