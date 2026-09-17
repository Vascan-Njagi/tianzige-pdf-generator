# 田字格生成器 - Tianzige PDF Generator

A C# WinForms application for generating customizable Chinese character writing grid PDFs.

## Features

- **Grid Types**: 田字格 (Tianzige), 米字格 (Mizige), 九宫格 (Jiugongge)
- **Page Types**: Cover, Grid, Lined, Blank, Ending
- **Page Sizes**: A3, A4, A5, B4, B5, Letter, Legal, Tabloid
- **Customization**: Cell size, spacing, margins, colors, line widths, dashed/solid guides
- **Live Preview**: Real-time visual feedback as you adjust settings
- **PDF Export**: High-quality output via QuestPDF + SkiaSharp
- **Project Save/Load**: JSON-based `.tzp` project files

## Prerequisites

### 1. Install .NET 10 SDK

Download from: https://dotnet.microsoft.com/download/dotnet/10.0

Verify: `dotnet --version` should show `10.0.xxx`

### 2. Install Visual Studio 2026 (or VS 2022 17.10+)

During installation, select the **".NET desktop development"** workload. This includes:
- .NET 10 SDK
- Windows Forms project templates
- NuGet package manager

### 3. NuGet Packages (auto-restored on first build)

| Package | Version | Purpose |
|---------|---------|---------|
| QuestPDF | 2024.10.3 | PDF generation (includes SkiaSharp) |

No manual download needed — `dotnet restore` or Visual Studio handles this automatically.

## Project Structure

```
tianzege/
├── TianzigeGenerator.sln           # Solution file (open this in VS)
├── .gitignore
├── README.md
└── TianzigeGenerator/
    ├── TianzigeGenerator.csproj     # Project file (net10.0-windows)
    ├── Program.cs                   # Entry point
    ├── MainForm.cs                  # Main window UI + event handlers
    ├── Models/
    │   ├── PageType.cs              # Enum: Cover, TianzigeGrid, MizigeGrid, etc.
    │   ├── PageSizeInfo.cs          # Standard page size definitions
    │   ├── GridStyle.cs             # Visual style (colors, line widths)
    │   ├── PageSettings.cs          # Per-page configuration
    │   └── Project.cs               # Multi-page document + JSON serialization
    ├── Controls/
    │   └── GridPreviewControl.cs    # Custom GDI+ live preview control
    └── Services/
        ├── GridRenderer.cs          # GDI+ renderer for preview
        └── PdfGenerator.cs          # QuestPDF/SkiaSharp PDF exporter
```

## How to Build & Run

### Option A: Visual Studio 2026

1. Double-click `TianzigeGenerator.sln` to open in Visual Studio
2. Wait for NuGet packages to restore (check Output window)
3. Press **F5** or click **▶ Start** to build and run

If packages don't restore automatically:
- Right-click solution → **Restore NuGet Packages**

### Option B: Command Line

```powershell
cd g:\Proj\tianzege\TianzigeGenerator
dotnet restore    # Download NuGet packages
dotnet build      # Compile
dotnet run        # Launch the app
```

### Option C: Publish Standalone EXE

```powershell
dotnet publish -c Release -r win-x64 --self-contained -o .\publish
```

This creates a standalone folder with everything needed to run (no .NET install required on target machine).

## Usage Guide

1. **Add Pages**: Click ➕ Add Page to add grid pages
2. **Configure**: Select a page, adjust settings in the right panel
3. **Grid Type**: Choose Tianzige, Mizige, or Jiugongge
4. **Cell Size**: Set in millimeters (typical: 15mm for practice)
5. **Style**: Customize colors, line widths, dashed/solid lines
6. **Cover/Ending**: Add title, subtitle, author name
7. **Reorder**: Use ⬆⬇ buttons to arrange pages
8. **Save Project**: 💾 Save as `.tzp` file
9. **Generate PDF**: 📄 Generate PDF to create the final document

## Troubleshooting

| Issue | Solution |
|-------|----------|
| `QuestPDF namespace not found` | Run `dotnet restore` |
| Chinese characters show as □ boxes | Install Chinese language fonts |
| Build fails with .NET version error | Ensure .NET 10 SDK is installed |
| PDF generation slow on first run | Normal — SkiaSharp initializes once |

## Technical Details

- **Framework**: .NET 10.0, Windows Forms
- **PDF Library**: QuestPDF 2024.10.3 (Community License)
- **Preview Rendering**: GDI+ (System.Drawing)
- **PDF Rendering**: SkiaSharp (via QuestPDF Canvas)
- **Serialization**: System.Text.Json
- **Fonts**: Microsoft YaHei (Chinese), Segoe UI (English)

