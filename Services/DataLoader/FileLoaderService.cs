using Hangfire;
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

            BackgroundJob.Enqueue(() => ProcessFileInBackground(fileItem.Id));

            return fileItem;
        }
        // TODO (07.11 - агент-векторизатор): добавить векторизацию здесь

        public void ProcessFileInBackground(Guid fileId)
        {
            var fileItem = _applicationDbContext.FileItems.Find(fileId);

            if (fileItem == null) throw new InvalidDataException();

            fileItem.Status = FileProcessingStatus.Parsing;
            _applicationDbContext.SaveChanges();

            try
            {
                var parser = _parserFactory.GetParser(fileItem.FileType);
                if (parser != null)
                {
                    var blocks = parser.Parse(fileItem);
                    _applicationDbContext.InfoBlocks.AddRange(blocks);
                    _applicationDbContext.SaveChanges();
                }
                else
                {
                    fileItem.Status = FileProcessingStatus.Failed;
                    fileItem.ErrorMessage = "Нет необходимого парсера!";
                    _applicationDbContext.SaveChanges();
                    return;
                }
            }
            catch(Exception ex)
            {
                fileItem.Status = FileProcessingStatus.Failed;
                fileItem.ErrorMessage = ex.Message;
                _applicationDbContext.SaveChanges();
                return;
            }
            fileItem.Status = FileProcessingStatus.Ready;
            _applicationDbContext.SaveChanges();
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
