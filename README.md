# 🤖 GZ-Coder

<div align="center">

**A RAG-Powered Code Assistant with Local LLM Models**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4?logo=blazor)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-336791?logo=postgresql)](https://www.postgresql.org/)
[![Qdrant](https://img.shields.io/badge/Qdrant-Vector_DB-DC244C)](https://qdrant.tech/)
[![License](https://img.shields.io/badge/License-Educational-green)](LICENSE)

[Features](#-features) • [Installation](#-installation) • [Usage](#-usage) • [Configuration](#-configuration) • [Architecture](#-architecture)

</div>

---

## 📖 Overview

GZ-Coder is a Retrieval-Augmented Generation (RAG) system designed to help developers understand and navigate codebases efficiently. Built as a university course project, it demonstrates the practical application of modern AI technologies in code analysis and documentation.

The system parses uploaded files into semantic blocks, generates embeddings using local models, stores them in a vector database, and provides intelligent answers to code-related queries through a conversational interface.

### 🎯 Key Capabilities

- **Semantic Code Search** - Find relevant code snippets based on natural language queries
- **Intelligent Code Explanation** - Get AI-generated explanations for complex code segments
- **Multi-Language Support** - Parse and analyze code from 15+ programming languages
- **Complete Privacy** - All processing happens locally with no external API calls

---

## ✨ Features

### 🔍 **Smart Code Understanding**
- Automatic extraction of classes, methods, functions, and code structures
- Context-aware parsing using TreeSitter for accurate syntax analysis
- Support for C#, Python, JavaScript, TypeScript, Java, C++, C, Rust, PHP, and more

### 🧠 **Local AI Models**
- Runs entirely offline using GGUF-format models
- BGE-M3 embedding model for semantic search
- Gemma 3 IT for natural language generation
- No data ever leaves your machine

### 💬 **Interactive Interface**
- Clean, modern Blazor Server UI
- Real-time chat interface for querying your codebase
- File upload with drag-and-drop support
- Live processing status updates

### ⚡ **Efficient Processing**
- Background job processing with Hangfire
- Asynchronous vectorization pipeline
- Incremental indexing of new files
- Fast semantic search with Qdrant vector database

---

## 🏗️ Architecture

```
┌─────────────┐
│   Upload    │
│   Files     │
└──────┬──────┘
       │
       ▼
┌─────────────────┐
│  File Parser    │──► TreeSitter (Code)
│  (Multi-format) │──► Markdig (Markdown)
└────────┬────────┘──► PdfPig (PDF)
         │           └► OpenXML (DOCX)
         ▼
┌─────────────────┐
│  Info Blocks    │
│  (PostgreSQL)   │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Vectorization  │──► BGE-M3 Model
│  (LMKit.NET)    │    (1024-dim)
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Qdrant Vector  │
│    Database     │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  User Query     │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Semantic Search │──► Retrieve Top-K
│  + RAG Context  │    Similar Blocks
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   Gemma 3 IT    │──► Generate Answer
│   (LLM Model)   │    with Context
└─────────────────┘
```

---

## 🚀 Installation

### Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/get-started) (for PostgreSQL and Qdrant)
- **Local LLM Models** (download separately):
  - 📊 Embedding Model: [bge-m3-Q4_K_M.gguf](https://huggingface.co/lm-kit/bge-m3-gguf/resolve/main/bge-m3-Q4_K_M.gguf?download=true) (~670 MB)
  - 💬 Generation Model: [gemma-3-it-1B-Q4_K_M.gguf](https://huggingface.co/lm-kit/gemma-3-1b-it-gguf/resolve/main/gemma-3-it-1B-Q4_K_M.gguf?download=true) (~715 MB)

### Setup Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/MrZetYT/GZ-Coder.git
   cd GZ-Coder
   ```

2. **Create models directory and add your models**
   ```bash
   mkdir LM
   # Move downloaded .gguf files to LM/ directory
   # Expected structure:
   # LM/
   # ├── bge-m3-Q4_K_M.gguf
   # └── gemma-3-it-1B-Q4_K_M.gguf
   ```

3. **Start database services**
   ```bash
   docker-compose up -d
   ```

4. **Configure database connection**
   
   Edit `appsettings.json` and update the password:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=raw_files;Username=postgres;Password=your_password_here"
     }
   }
   ```

5. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

6. **Run the application**
   ```bash
   dotnet run
   ```

7. **Access the application**
   
   Open your browser and navigate to: `https://localhost:7007`

---

## 📚 Usage

### 1️⃣ Upload Files

Navigate to the home page and upload your code files:
- Click **"Choose File"** or drag and drop files
- Supports multiple file upload
- Maximum file size: **50 MB** per file

**Supported formats:**
- **Code**: `.cs`, `.py`, `.js`, `.ts`, `.java`, `.cpp`, `.c`, `.rs`, `.php`
- **Web**: `.html`, `.css`
- **Documents**: `.pdf`, `.docx`, `.md`, `.txt`, `.rtf`

### 2️⃣ Wait for Processing

Files go through automatic processing:
- **Parsing** - Code is analyzed and split into logical blocks
- **Vectorization** - Each block is converted to embeddings
- **Status updates** every 3 seconds

Wait for the ✅ **Ready** status before querying.

### 3️⃣ Ask Questions

Navigate to the **Chat** section and ask questions like:
- *"What does the SaveFiles method do?"*
- *"How is vectorization implemented?"*
- *"Show me authentication-related code"*
- *"Explain the database connection logic"*

The system will:
1. Search for semantically similar code blocks
2. Retrieve top matches with similarity scores
3. Generate an explanation using the LLM with context

---

## ⚙️ Configuration

### Using Custom Models

To use different GGUF models, update the following files:

#### Embedding Model
**File:** `Services/Vectorization/VectorizationService.cs` (line 17)

```csharp
var modelPath = Path.Combine(Directory.GetCurrentDirectory(), 
    "LM", "your-embedding-model.gguf");
```

**Requirements:**
- Must produce **1024-dimensional** vectors
- Or update `VectorSize` in `Services/VectorStorage/VectorStorageService.cs` (line 13)

#### Text Generation Model
**File:** `Services/Explanation/ExplanationService.cs` (line 27)

```csharp
var modelPath = Path.Combine(Directory.GetCurrentDirectory(), 
    "LM", "your-generation-model.gguf");
```

### Database Configuration

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=raw_files;Username=postgres;Password=your_password"
  },
  "Qdrant": {
    "Host": "localhost",
    "Port": 6334
  }
}
```

Make sure the password matches in both `appsettings.json` and `docker-compose.yml`.

---

## 🗂️ Project Structure

```
GZ-Coder/
├── 📁 Controllers/              # API endpoints
│   ├── ExplanationController.cs # Question answering API
│   ├── FileLoaderController.cs  # File upload/management
│   ├── SearchController.cs      # Vector search API
│   └── VectorizationController.cs
│
├── 📁 Services/
│   ├── 📁 DataLoader/           # File handling
│   │   ├── FileLoaderService.cs
│   │   ├── FileTypeResolver.cs
│   │   └── FileValidator.cs
│   │
│   ├── 📁 Parsers/               # Language parsers
│   │   ├── CSharpParser.cs
│   │   ├── MarkdownParser.cs
│   │   ├── PdfParser.cs
│   │   ├── DocxParser.cs
│   │   └── TreeSitterParsers/   # Multi-language support
│   │
│   ├── 📁 Vectorization/         # Embedding generation
│   │   └── VectorizationService.cs
│   │
│   ├── 📁 VectorStorage/         # Qdrant integration
│   │   └── VectorStorageService.cs
│   │
│   └── 📁 Explanation/           # LLM response generation
│       └── ExplanationService.cs
│
├── 📁 Pages/                     # Blazor UI
│   ├── FileList.razor            # File management page
│   ├── Chat.razor                # Chat interface
│   └── App.razor
│
├── 📁 Models/                    # Data models
│   ├── FileItem.cs
│   ├── InfoBlock.cs
│   └── QdrantModels.cs
│
├── 📁 Database/                  # EF Core
│   ├── ApplicationDbContext.cs
│   └── ApplicationDbContextFactory.cs
│
├── 📁 Migrations/                # Database migrations
├── 📁 Configs/                   # Configuration files
├── 📁 LM/                        # 🔴 Place models here (create manually)
├── 📄 docker-compose.yml         # Docker setup
├── 📄 appsettings.json          # App configuration
└── 📄 Program.cs                # Application entry point
```

---

## 🛠️ Technology Stack

| Component | Technology | Purpose |
|-----------|------------|---------|
| **Backend** | ASP.NET Core 9.0 | Web framework |
| **UI** | Blazor Server | Interactive web interface |
| **Database** | PostgreSQL 17 | Metadata storage |
| **Vector DB** | Qdrant | Semantic search |
| **LLM Runtime** | LMKit.NET | Local model inference |
| **Code Parsing** | TreeSitter, Roslyn | Multi-language parsing |
| **Job Queue** | Hangfire | Background processing |
| **ORM** | Entity Framework Core | Database operations |
| **Document Processing** | PdfPig, OpenXML, Markdig | File parsing |

---

## 👥 Authors

This project was developed as a university course project by:

- Gesman Nikita **[@MrZetYT](https://github.com/MrZetYT)**
- Maxim Zinovich **[@Belochka228](https://github.com/Belochka228)**

---

## 🤝 Contributing

Contributions, issues, and feature requests are welcome!

---

## 📄 License

This project is available for educational and experimental purposes. See the repository for details.

---

## 🙏 Acknowledgments

Special thanks to:
- **[LMKit.NET](https://github.com/LM-Kit/lm-kit.net)** - For enabling local LLM inference in .NET
- **[TreeSitter](https://tree-sitter.github.io/tree-sitter/)** - For robust multi-language code parsing
- **[Qdrant](https://qdrant.tech/)** - For efficient vector search capabilities
- **[Hangfire](https://www.hangfire.io/)** - For reliable background job processing

---

<div align="center">

**Built with ❤️ for developers who value privacy and local-first AI**

⭐ Star this repository if you found it helpful!

</div>
