using Microsoft.EntityFrameworkCore;
using RAG_Code_Base.Models;

namespace RAG_Code_Base.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FileItem> FileItems { get; set; }
        public DbSet<InfoBlock> InfoBlocks { get; set; }
    }
}