﻿using Hangfire;
using Microsoft.EntityFrameworkCore;
using RAG_Code_Base.Database;
using RAG_Code_Base.Models;
using RAG_Code_Base.Services.Parsers;
using RAG_Code_Base.Services.Vectorization;
using RAG_Code_Base.Services.VectorStorage;

namespace RAG_Code_Base.Services.DataLoader
{
    public class FileLoaderService
    {
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        private readonly ILogger<FileLoaderService> _logger;
        private readonly FileTypeResolver _typeResolver = new();
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ParserFactory _parserFactory;
        private readonly VectorizationService _vectorizationService;
        private readonly VectorStorageService _vectorStorageService;
        public FileLoaderService(ApplicationDbContext applicationDbContext, ParserFactory parserFactory,
            VectorizationService vectorizationService, VectorStorageService vectorStorageService, ILogger<FileLoaderService> logger)
        {
            _applicationDbContext = applicationDbContext;
            _parserFactory = parserFactory;
            _vectorizationService = vectorizationService;
            _vectorStorageService = vectorStorageService;
            if (!Directory.Exists(_storagePath))
                Directory.CreateDirectory(_storagePath);
            _logger = logger;
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


            System.Threading.Thread.Sleep(5000);


            try
            {
                var parser = _parserFactory.GetParser(fileItem.FileType);
                if (parser != null)
                {
                    var blocks = parser.Parse(fileItem);
                    _applicationDbContext.InfoBlocks.AddRange(blocks);
                    _applicationDbContext.SaveChanges();

                    



                    fileItem.Status = FileProcessingStatus.Vectorizing;
                    foreach( var block in blocks)
                    {
                        BackgroundJob.Enqueue(() => VectorizeBlockInBackgroundAsync(block.Id));
                    }

             

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
            _applicationDbContext.SaveChanges();
        }

        public async Task VectorizeBlockInBackgroundAsync(Guid infoBlockId)
        {
            var infoBlock = _applicationDbContext.InfoBlocks
                .Include(n=> n.FileItem).FirstOrDefault(id => id.Id == infoBlockId);

            if (infoBlock == null) throw new InvalidDataException();
            try
            {
                var vector = await _vectorizationService.GenerateEmbeddingAsync(infoBlock.Content);
                await _vectorStorageService.SaveEmbeddingAsync(infoBlockId, vector);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка векторизации блока {BlockId} файла {FileId}", infoBlockId, infoBlock.FileItemId);
                return;
            }

            infoBlock.IsVectorized = true;
            _applicationDbContext.SaveChanges();
            
            var unprocessedCount = _applicationDbContext.InfoBlocks.Count(b => b.FileItemId == infoBlock.FileItemId && !b.IsVectorized);
            if(unprocessedCount == 0)
            {
                infoBlock.FileItem.Status = FileProcessingStatus.Ready;
                _applicationDbContext.SaveChanges();
            }
        }

        public List<FileItem> GetAllFiles()
        {
            return _applicationDbContext.FileItems.ToList();
        }

        public bool DeleteFile(Guid id)
        {
            var fileItem = _applicationDbContext.FileItems
                .Include(f => f.InfoBlocks)  // <-- добавили загрузку связанных блоков
                .FirstOrDefault(f => f.Id == id);

            if (fileItem == null)
            {
                return false;
            }

            // Удаляем физический файл
            if (File.Exists(fileItem.FilePath))
            {
                File.Delete(fileItem.FilePath);
            }

            // Удаляем связанные InfoBlocks (каскадное удаление через EF)
            // Благодаря Include они уже загружены и удалятся автоматически
            _applicationDbContext.FileItems.Remove(fileItem);
            _applicationDbContext.SaveChanges();

            return true;
        }
    }
}
