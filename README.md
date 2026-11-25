Tianzige PDF Generator
A modern .NET 10 WinForms desktop application for generating and customizing Chinese calligraphy grids, vocabulary practice sheets, essay manuscript pages, lined notebooks, and custom cover pages for PDF export.

src/TianzigeGenerator/
├── TianzigeGenerator.csproj     # .NET 10 Windows Forms project file
├── Program.cs                   # Application entry point
├── MainForm.cs                  # Main window, event handlers & responsive splitter layout
├── Models/
│   ├── PageType.cs              # Enum: TianzigeGrid, JiugonggeGrid, EssayGrid, Vocabulary, Lined, Blank, Custom
│   ├── PageSizeInfo.cs          # Standard paper sizes (A4, A3, B5, Letter, etc.)
│   ├── GridStyle.cs             # Styling settings (colors, borders, guide dash patterns, corner radius)
│   ├── PageSettings.cs          # Per-page settings, margins, headers, text blocks, and dimensions
│   ├── CustomElement.cs         # Base model for WYSIWYG elements
│   ├── CustomTextElement.cs     # Text block element model
│   ├── CustomImageElement.cs    # Image element model
│   └── Project.cs               # Multi-page project document model + JSON serialization
├── Controls/
│   └── GridPreviewControl.cs    # Double-buffered GDI+ live preview control
└── Services/
    ├── GridRenderer.cs          # GDI+ renderer for live preview
    ├── PdfGenerator.cs          # QuestPDF + SkiaSharp PDF exporter
    └── QuestPdfSkiaExtensions.cs # Canvas extension for SkiaSharp vector drawing inside QuestPDF


##  How to Build & Run

### Prerequisites
1. **.NET 10 SDK**: [Download .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0)
2. **Visual Studio 2026** (or VS 2022 17.10+) with the **.NET desktop development** workload.

### Command Line
```powershell
# Restore NuGet dependencies
dotnet restore

# Build project
dotnet build "src/TianzigeGenerator/TianzigeGenerator.csproj"

# Launch app
dotnet run --project "src/TianzigeGenerator/TianzigeGenerator.csproj"
```

### Visual Studio
1. Open `TianzigeGenerator.sln`.
2. Press **F5** or click **▶ Start**.

- **Target Framework**: .NET 10.0 (`net10.0-windows`)
- **UI Framework**: Windows Forms (WinForms) with custom GDI+ controls & `SplitContainer` panels
- **PDF Generation**: QuestPDF (2024.10.3)
- **Vector Rendering**: SkiaSharp (2.88.8)
- **Serialization**: `System.Text.Json`
