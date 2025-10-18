namespace RAG_Code_Base.Models
{
    public enum FileProcessingStatus
    {
        Uploaded,
        Parsing,
        Vectorizing,
        Ready,
        Failed
    }
    public class FileItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public FileProcessingStatus Status { get; set; } = FileProcessingStatus.Uploaded;
        public string? ErrorMessage { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public List<InfoBlock> InfoBlocks { get; set; } = new();
    }
}
