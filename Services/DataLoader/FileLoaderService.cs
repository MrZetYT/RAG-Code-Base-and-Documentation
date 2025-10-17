using RAG_Code_Base.Database;
using RAG_Code_Base.Models;
using RAG_Code_Base.Services.Parsers;

namespace RAG_Code_Base.Services.DataLoader
{
    public class FileLoaderService
    {
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        private readonly FileTypeResolver _typeResolver = new();
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ParserFactory _parserFactory;
        public FileLoaderService(ApplicationDbContext applicationDbContext, ParserFactory parserFactory)
        {
            _applicationDbContext = applicationDbContext;
            _parserFactory = parserFactory;
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

            var parser = _parserFactory.GetParser(fileItem.FileType);
            if(parser != null)
            {
                var blocks = parser.Parse(fileItem);
                _applicationDbContext.InfoBlocks.AddRange(blocks);
                _applicationDbContext.SaveChanges();
            }

            return fileItem;
        }

        public List<FileItem> GetAllFiles()
        {
            return _applicationDbContext.FileItems.ToList();
        }

        public bool DeleteFile(Guid id)
        {
            var fileItem = _applicationDbContext.FileItems.FirstOrDefault(f=>f.Id== id);

            if (fileItem == null)
            {
                return false;
            }

            if (File.Exists(fileItem.FilePath))
            {
                File.Delete(fileItem.FilePath);
            }

            _applicationDbContext.FileItems.Remove(fileItem);
            _applicationDbContext.SaveChanges();


            return true;
        }
    }
}
