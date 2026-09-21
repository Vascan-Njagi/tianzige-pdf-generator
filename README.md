# 田字格生成器 - Tianzige PDF Generator

A modern .NET 10 WinForms desktop application for generating and customizing Chinese calligraphy grids, vocabulary practice sheets, essay manuscript pages, lined notebooks, and custom cover pages for PDF export.

---

## 🌟 Up-to-Date Features

### 📐 Grid & Page Types
- **Tianzige (田字格)**: Classic 2x2 cross guide lines for character balance.
- **Jiugongge (九宫格)**: 3x3 9-cell grid for detailed character stroke proportions.
- **Essay Grid (作文格)**: Continuous manuscript grid layout for writing essays.
- **Vocabulary Sheets (生字本)**: Grid cells paired with Pinyin top boxes and lined translation spaces.
- **Lined Notebook (横线本)**: Customizable line spacing, line weight, line color, and optional vertical margin rule.
- **Blank & Custom Pages**: Cover/ending templates with customizable title, subtitle, author, notes, and cover image upload.

### 📐 Dimensions & Layout Control
- **Page Sizes**: A3, A4, A5, B4, B5, Letter, Legal, Tabloid, and Custom dimensions.
- **Orientation**: Instant toggle between Portrait and Landscape modes.
- **Margins & Spacing**: Millimeter-precision controls for Top, Bottom, Left, Right margins, cell size, cell gap, row gap, and column gap.
- **Overrides & Headers**: Custom column headers (text above grid columns) and explicit Row/Column overrides.

### 🎨 Visual Styling & Theming
- **Guides**: Choose Dashed, Dotted, or Solid guide lines with custom guide colors and stroke widths.
- **Diagonals (米字格)**: Toggle diagonal guides on grid cells.
- **Borders & Corners**: Outer cell border colors, line thickness, rounded cell corner radius, and cell inner padding.
- **Color Customization**: Customizable page background, cell background, guide colors, line colors, and optional cell shading tint.

### 🖥️ Interactive Desktop UI
- **Resizable 3-Panel Splitter**: Drag splitters between **Page Settings**, **Live Preview**, and **Pages Tree** with app-wide minimum size safeguards.
- **GDI+ Live Preview**: Smooth, real-time rendering of grid pages as settings change.
- **Page Management**: Add, duplicate, reorder (move up/down), remove, multiply pages, or apply current page settings across all pages in the document.
- **Dynamic Text Blocks**: Add, remove, and position dynamic text blocks on custom/cover pages with font size and gap controls.

### 📄 Export & Persistence
- **PDF Export**: Vector-quality PDF documents generated via **QuestPDF** and **SkiaSharp**.
- **Project Files**: Save and load complete multi-page document projects using JSON `.tzp` project files.

---

## 🚧 Unfinished Stuff & Work in Progress

The following features and improvements are actively being worked on:

- [ ] **Responsive Page Actions Layout**: Improved auto-wrapping and percentage-based scaling for global page action buttons on narrower panels.
- [ ] **Character & Stroke Order Overlay (描红 / 字帖)**: Ability to print gray stroke-order tracing characters directly inside grid cells for writing practice.
- [ ] **Vertical Chinese Reading Layout**: Right-to-left vertical column manuscript layout for traditional Chinese essay sheets.
- [ ] **Page Numbering & Headers/Footers**: Automatic dynamic page numbers (`Page X of Y`), headers, and footers across multi-page PDF exports.
- [ ] **Rich Text Formatting in Text Blocks**: Per-block font family selection, alignment (Left/Center/Right), text color, and bold/italic toggles for custom text blocks.
- [ ] **Preset Template Library**: Quick-start design presets (e.g. Primary School Practice, HSK Vocabulary, Calligraphy Examination).

---

## 🛠️ Project Structure

```
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
```

---

## 🚀 How to Build & Run

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

---

## 📋 Usage Guide

1. **Add & Select Pages**: Click **➕ Add Page** in the right-hand **Pages** panel to create pages.
2. **Configure Layout**: Switch between **Layout**, **Grid / Lines**, **Style**, and **Cover/Ending** tabs in the **Page Settings** panel.
3. **Adjust Proportions**: Drag the vertical splitter bars between panels to customize panel widths to your preference.
4. **Save Project**: Use **💾 Save Project** to store your document as a `.tzp` file.
5. **Export PDF**: Click **📄 Generate PDF** to render a PDF document.

---

## 🧰 Technical Stack

- **Target Framework**: .NET 10.0 (`net10.0-windows`)
- **UI Framework**: Windows Forms (WinForms) with custom GDI+ controls & `SplitContainer` panels
- **PDF Generation**: QuestPDF (2024.10.3)
- **Vector Rendering**: SkiaSharp (2.88.8)
- **Serialization**: `System.Text.Json`
