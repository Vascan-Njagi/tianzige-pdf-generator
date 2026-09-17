using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TianzigeGenerator.Models;
using Color = QuestPDF.Infrastructure.Color;

namespace TianzigeGenerator.Services;

public static class PdfGenerator
{
    public static void Generate(Project project, string outputPath)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
        {
            foreach (var page in project.Pages)
            {
                container.Page(p => ConfigurePage(p, page));
            }
        })
        .GeneratePdf(outputPath);
    }

    private static void ConfigurePage(PageDescriptor page, PageSettings settings)
    {
        float w = settings.EffectiveWidthMm;
        float h = settings.EffectiveHeightMm;
        page.Size(new PageSize(w, h));
        page.MarginTop(settings.MarginTopMm);
        page.MarginBottom(settings.MarginBottomMm);
        page.MarginLeft(settings.MarginLeftMm);
        page.MarginRight(settings.MarginRightMm);

        var bgColor = ToQColor(settings.Style.PageBackground);
        page.PageColor(bgColor);

        // Calculate available content area after margins
        float contentWidth = w - settings.MarginLeftMm - settings.MarginRightMm;
        float contentHeight = h - settings.MarginTopMm - settings.MarginBottomMm;

        page.Content().Column(col =>
        {
            switch (settings.Type)
            {
                case PageType.TianzigeGrid:
                case PageType.MizigeGrid:
                case PageType.JiugonggeGrid:
                    col.Item().Height(1).ExtendHorizontal().Background(bgColor);
                    col.Item().ExtendHorizontal().Grid(grid => DrawGridFluent(grid, settings, contentWidth, contentHeight));
                    break;
                case PageType.Cover:
                    col.Item().ExtendHorizontal().Column(coverCol => DrawCoverFluent(coverCol, settings));
                    break;
                case PageType.Ending:
                    col.Item().ExtendHorizontal().Column(endingCol => DrawEndingFluent(endingCol, settings));
                    break;
                case PageType.Lined:
                    col.Item().Height(1).ExtendHorizontal().Background(bgColor);
                    col.Item().ExtendHorizontal().Column(linedCol => DrawLinedFluent(linedCol, settings, contentHeight));
                    break;
                case PageType.Blank:
                    col.Item().Height(1).ExtendHorizontal().Background(bgColor);
                    break;
            }
        });
    }

    private static void DrawGridFluent(GridDescriptor grid, PageSettings s, float contentWidth, float contentHeight)
    {
        float cellSize = s.CellSizeMm;
        float colSpacing = s.ColumnSpacingMm;
        float rowSpacing = s.LineSpacingMm;
        if (cellSize <= 0) return;

        // Calculate grid dimensions based on available content area or override
        int cols, rows;
        if (s.GridColsOverride > 0)
            cols = s.GridColsOverride;
        else
            cols = Math.Max(1, (int)((contentWidth + colSpacing) / (cellSize + colSpacing)));
        
        if (s.GridRowsOverride > 0)
            rows = s.GridRowsOverride;
        else
            rows = Math.Max(1, (int)((contentHeight + rowSpacing) / (cellSize + rowSpacing)));

        var borderColor = ToQColor(s.Style.BorderColor);
        var fillColor = ToQColor(s.Style.CellBackground);
        var guideColor = ToQColor(s.Style.GuideColor);
        float borderWidth = s.Style.BorderWidthPt;
        float guideWidth = s.Style.GuideWidthPt;
        bool dashedGuides = s.Style.DashedGuides;
        bool dottedGuides = s.Style.DottedGuides;

        for (int r = 0; r < rows; r++)
        {
            grid.Columns(cols + 1);
            for (int c = 0; c <= cols; c++)
            {
                if (c == 0)
                {
                    grid.Item().Height(cellSize);
                }
                else
                {
                    // Create cell with border and guide lines using SVG
                    var svgContent = GenerateCellSvg(s, guideColor, guideWidth, dashedGuides, dottedGuides);
                    grid.Item().Background(fillColor).Border(borderWidth).BorderColor(borderColor).Svg(svgContent);
                }
            }
        }
    }

    private static string GenerateCellSvg(PageSettings s, Color guideColor, float guideWidth, bool dashedGuides, bool dottedGuides)
    {
        var strokeColor = $"rgb({guideColor.Red},{guideColor.Green},{guideColor.Blue})";
        var strokeWidth = guideWidth;
        string dashArray = "";
        if (dottedGuides)
        {
            dashArray = " stroke-dasharray=\"1,2\" stroke-linecap=\"round\"";
        }
        else if (dashedGuides)
        {
            dashArray = " stroke-dasharray=\"3,2\"";
        }
        
        string lines = "";
        
        if (s.Type == PageType.TianzigeGrid || s.Type == PageType.MizigeGrid)
        {
            // Horizontal center line
            lines += $"<line x1=\"0\" y1=\"50\" x2=\"100\" y2=\"50\" stroke=\"{strokeColor}\" stroke-width=\"{strokeWidth}\"{dashArray}/>";
            // Vertical center line
            lines += $"<line x1=\"50\" y1=\"0\" x2=\"50\" y2=\"100\" stroke=\"{strokeColor}\" stroke-width=\"{strokeWidth}\"{dashArray}/>";
        }

        if (s.Type == PageType.MizigeGrid && s.Style.DrawDiagonals)
        {
            // Diagonal lines
            lines += $"<line x1=\"0\" y1=\"0\" x2=\"100\" y2=\"100\" stroke=\"{strokeColor}\" stroke-width=\"{strokeWidth}\"{dashArray}/>";
            lines += $"<line x1=\"100\" y1=\"0\" x2=\"0\" y2=\"100\" stroke=\"{strokeColor}\" stroke-width=\"{strokeWidth}\"{dashArray}/>";
        }

        if (s.Type == PageType.JiugonggeGrid)
        {
            // Vertical lines at 1/3 and 2/3
            lines += $"<line x1=\"33.33\" y1=\"0\" x2=\"33.33\" y2=\"100\" stroke=\"{strokeColor}\" stroke-width=\"{strokeWidth}\"{dashArray}/>";
            lines += $"<line x1=\"66.67\" y1=\"0\" x2=\"66.67\" y2=\"100\" stroke=\"{strokeColor}\" stroke-width=\"{strokeWidth}\"{dashArray}/>";
            // Horizontal lines at 1/3 and 2/3
            lines += $"<line x1=\"0\" y1=\"33.33\" x2=\"100\" y2=\"33.33\" stroke=\"{strokeColor}\" stroke-width=\"{strokeWidth}\"{dashArray}/>";
            lines += $"<line x1=\"0\" y1=\"66.67\" x2=\"100\" y2=\"66.67\" stroke=\"{strokeColor}\" stroke-width=\"{strokeWidth}\"{dashArray}/>";
        }

        return $"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 100 100\" width=\"100%\" height=\"100%\">{lines}</svg>";
    }

    private static void DrawCoverFluent(ColumnDescriptor col, PageSettings s)
    {
        var borderColor = ToQColor(s.Style.BorderColor);

        col.Spacing(20);
        col.Item().Height(40);

        col.Item().Text(s.Title).FontSize(s.TitleFontSizePt).Bold().FontColor(borderColor).AlignCenter();

        if (!string.IsNullOrEmpty(s.Subtitle))
        {
            col.Item().Text(s.Subtitle).FontSize(s.SubtitleFontSizePt).Italic().FontColor(borderColor).AlignCenter();
        }

        if (!string.IsNullOrEmpty(s.AuthorName))
        {
            col.Item().Text($"姓名: {s.AuthorName}").FontSize(s.AuthorFontSizePt).FontColor(borderColor).AlignCenter();
        }

        if (!string.IsNullOrEmpty(s.Notes))
        {
            col.Item().Text(s.Notes).FontSize(s.NotesFontSizePt).FontColor(borderColor).AlignCenter();
        }

        col.Item().Height(40);
    }

    private static void DrawEndingFluent(ColumnDescriptor col, PageSettings s)
    {
        var borderColor = ToQColor(s.Style.BorderColor);

        col.Spacing(20);
        col.Item().Height(40);

        col.Item().Text(s.Title).FontSize(s.TitleFontSizePt).Bold().FontColor(borderColor).AlignCenter();

        if (!string.IsNullOrEmpty(s.Notes))
        {
            col.Item().Text(s.Notes).FontSize(s.NotesFontSizePt).FontColor(borderColor).AlignCenter();
        }

        col.Item().Height(40);
    }

    private static void DrawLinedFluent(ColumnDescriptor col, PageSettings s, float contentHeight)
    {
        float lineSpacing = s.LineHeightMm;
        if (lineSpacing <= 0) return;

        var lineColor = ToQColor(s.LineColor);

        col.Spacing(lineSpacing);
        int lines = (int)(contentHeight / lineSpacing);
        for (int i = 0; i < lines; i++)
        {
            col.Item().LineHorizontal(0.5f).LineColor(lineColor);
        }
    }

    private static Color ToQColor(System.Drawing.Color c) => Color.FromHex($"{c.R:X2}{c.G:X2}{c.B:X2}");
}
