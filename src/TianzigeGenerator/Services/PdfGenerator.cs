using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using TianzigeGenerator.Models;

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
        const float mmToPt = 72f / 25.4f;

        float wPt = settings.EffectiveWidthMm * mmToPt;
        float hPt = settings.EffectiveHeightMm * mmToPt;

        page.Size(new PageSize(wPt, hPt));

        var bgColor = QuestPDF.Infrastructure.Color.FromHex($"{settings.Style.PageBackground.R:X2}{settings.Style.PageBackground.G:X2}{settings.Style.PageBackground.B:X2}");
        page.PageColor(bgColor);

        // Explicitly set the margins in Points
        page.MarginTop(settings.MarginTopMm * mmToPt);
        page.MarginBottom(settings.MarginBottomMm * mmToPt);
        page.MarginLeft(settings.MarginLeftMm * mmToPt);
        page.MarginRight(settings.MarginRightMm * mmToPt);

        // Draw full-page borders for Cover and Ending pages removed for now

        // Now populate the Content area (which strictly respects the margins)
        page.Content().Element(contentContainer =>
        {
            switch (settings.Type)
            {
                case PageType.TianzigeGrid:
                case PageType.MizigeGrid:
                case PageType.JiugonggeGrid:
                case PageType.EssayGrid:
                    contentContainer.SkiaSharpSvgCanvas((canvas, space) => DrawGridFluent(canvas, settings, space));
                    break;
                case PageType.Vocabulary:
                    contentContainer.SkiaSharpSvgCanvas((canvas, space) => DrawVocabularyFluent(canvas, settings, space));
                    break;
                case PageType.Custom:
                    contentContainer.Padding(40, Unit.Point).Column(col => DrawCustomPageFluent(col, settings));
                    break;
                case PageType.Lined:
                    contentContainer.SkiaSharpSvgCanvas((canvas, space) => DrawLinedFluent(canvas, settings, space));
                    break;
                case PageType.Blank:
                    break;
            }
        });
    }

    private static void DrawGridFluent(SKCanvas canvas, PageSettings s, SKSize space)
    {
        const float mmToPt = 72f / 25.4f;
        float cellSizePt = s.CellSizeMm * mmToPt;
        float colSpacingPt = s.ColumnSpacingMm * mmToPt;
        float rowSpacingPt = s.LineSpacingMm * mmToPt;
        if (cellSizePt <= 0) return;

        int cols = s.GridColsOverride > 0 ? s.GridColsOverride : Math.Max(1, (int)((space.Width + colSpacingPt) / (cellSizePt + colSpacingPt)));
        int rows = s.GridRowsOverride > 0 ? s.GridRowsOverride : Math.Max(1, (int)((space.Height + rowSpacingPt) / (cellSizePt + rowSpacingPt)));

        float totalWidth = cols * cellSizePt + (cols - 1) * colSpacingPt;
        float totalHeight = rows * cellSizePt + (rows - 1) * rowSpacingPt;

        float startX = (space.Width - totalWidth) / 2f;
        float startY = (space.Height - totalHeight) / 2f;

        using var outerPen = new SKPaint { Color = new SKColor(s.Style.BorderColor.R, s.Style.BorderColor.G, s.Style.BorderColor.B), Style = SKPaintStyle.Stroke, StrokeWidth = s.Style.BorderWidthPt, IsAntialias = true };
        using var fillBrush = new SKPaint { Color = new SKColor(s.Style.CellBackground.R, s.Style.CellBackground.G, s.Style.CellBackground.B), Style = SKPaintStyle.Fill, IsAntialias = true };
        using var guidePen = new SKPaint { Color = new SKColor(s.Style.GuideColor.R, s.Style.GuideColor.G, s.Style.GuideColor.B), Style = SKPaintStyle.Stroke, StrokeWidth = s.Style.GuideWidthPt, IsAntialias = true };

        if (s.Style.DottedGuides)
        {
            guidePen.PathEffect = SKPathEffect.CreateDash(new float[] { 2f, 4f }, 0);
            guidePen.StrokeCap = SKStrokeCap.Round;
        }
        else if (s.Style.DashedGuides)
        {
            guidePen.PathEffect = SKPathEffect.CreateDash(new float[] { s.Style.DashPattern[0] * 2, s.Style.DashPattern[1] * 2 }, 0);
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                float x = startX + c * (cellSizePt + colSpacingPt);
                float y = startY + r * (cellSizePt + rowSpacingPt);

                var rect = new SKRect(x, y, x + cellSizePt, y + cellSizePt);

                if (s.Style.CellBackground.R != 255 || s.Style.CellBackground.G != 255 || s.Style.CellBackground.B != 255)
                {
                    canvas.DrawRect(rect, fillBrush);
                }

                float midX = rect.MidX;
                float midY = rect.MidY;

                if (s.Type == PageType.TianzigeGrid || s.Type == PageType.MizigeGrid)
                {
                    canvas.DrawLine(rect.Left, midY, rect.Right, midY, guidePen);
                    canvas.DrawLine(midX, rect.Top, midX, rect.Bottom, guidePen);
                }

                if (s.Type == PageType.MizigeGrid && s.Style.DrawDiagonals)
                {
                    canvas.DrawLine(rect.Left, rect.Top, rect.Right, rect.Bottom, guidePen);
                    canvas.DrawLine(rect.Right, rect.Top, rect.Left, rect.Bottom, guidePen);
                }

                if (s.Type == PageType.JiugonggeGrid)
                {
                    float stepX = rect.Width / 3f;
                    float stepY = rect.Height / 3f;
                    canvas.DrawLine(rect.Left + stepX, rect.Top, rect.Left + stepX, rect.Bottom, guidePen);
                    canvas.DrawLine(rect.Left + 2 * stepX, rect.Top, rect.Left + 2 * stepX, rect.Bottom, guidePen);
                    canvas.DrawLine(rect.Left, rect.Top + stepY, rect.Right, rect.Top + stepY, guidePen);
                    canvas.DrawLine(rect.Left, rect.Top + 2 * stepY, rect.Right, rect.Top + 2 * stepY, guidePen);
                }

                // Note: EssayGrid intentionally has no inner lines

                canvas.DrawRect(rect, outerPen);
            }
        }
    }

    private static void DrawVocabularyFluent(SKCanvas canvas, PageSettings s, SKSize space)
    {
        const float mmToPt = 72f / 25.4f;
        float cellSizePt = s.CellSizeMm * mmToPt;
        float pinyinHeightPt = s.PinyinHeightMm * mmToPt;
        float colSpacingPt = s.ColumnSpacingMm * mmToPt;
        float rowSpacingPt = s.LineSpacingMm * mmToPt;
        if (cellSizePt <= 0) return;

        int cols = s.GridColsOverride > 0 ? s.GridColsOverride : Math.Max(1, (int)(space.Width * 0.4f / (cellSizePt + colSpacingPt)));
        int rows = s.GridRowsOverride > 0 ? s.GridRowsOverride : Math.Max(1, (int)((space.Height + rowSpacingPt) / (cellSizePt + pinyinHeightPt + rowSpacingPt)));

        float totalGridW = cols * cellSizePt + (cols - 1) * colSpacingPt;
        float totalGridH = rows * (cellSizePt + pinyinHeightPt) + (rows - 1) * rowSpacingPt;

        float startX = 0; // Align left
        float startY = (space.Height - totalGridH) / 2f; // Center vertically

        using var outerPen = new SKPaint { Color = new SKColor(s.Style.BorderColor.R, s.Style.BorderColor.G, s.Style.BorderColor.B), Style = SKPaintStyle.Stroke, StrokeWidth = s.Style.BorderWidthPt, IsAntialias = true };
        using var linePen = new SKPaint { Color = new SKColor(s.LineColor.R, s.LineColor.G, s.LineColor.B), Style = SKPaintStyle.Stroke, StrokeWidth = s.LineWidthPt, IsAntialias = true };
        using var guidePen = new SKPaint { Color = new SKColor(s.Style.GuideColor.R, s.Style.GuideColor.G, s.Style.GuideColor.B), Style = SKPaintStyle.Stroke, StrokeWidth = s.Style.GuideWidthPt, IsAntialias = true };

        if (s.Style.DashedGuides) { guidePen.PathEffect = SKPathEffect.CreateDash(new float[] { s.Style.DashPattern[0] * 2, s.Style.DashPattern[1] * 2 }, 0); }

        for (int r = 0; r < rows; r++)
        {
            float rowY = startY + r * (cellSizePt + pinyinHeightPt + rowSpacingPt);

            for (int c = 0; c < cols; c++)
            {
                float x = startX + c * (cellSizePt + colSpacingPt);

                // Draw Pinyin Box
                var pinyinRect = new SKRect(x, rowY, x + cellSizePt, rowY + pinyinHeightPt);
                canvas.DrawRect(pinyinRect, outerPen);

                // Draw Character Box
                var cellRect = new SKRect(x, rowY + pinyinHeightPt, x + cellSizePt, rowY + pinyinHeightPt + cellSizePt);
                canvas.DrawRect(cellRect, outerPen);

                // Draw Cross Guides
                canvas.DrawLine(cellRect.Left, cellRect.MidY, cellRect.Right, cellRect.MidY, guidePen);
                canvas.DrawLine(cellRect.MidX, cellRect.Top, cellRect.MidX, cellRect.Bottom, guidePen);
            }

            // Draw Translation Lines
            float linesStartX = startX + totalGridW + (10f * mmToPt);
            float linesW = space.Width - totalGridW - (10f * mmToPt);

            float line1Y = rowY + pinyinHeightPt + (cellSizePt * 0.33f);
            float line2Y = rowY + pinyinHeightPt + (cellSizePt * 0.66f);

            canvas.DrawLine(linesStartX, line1Y, linesStartX + linesW, line1Y, linePen);
            canvas.DrawLine(linesStartX, line2Y, linesStartX + linesW, line2Y, linePen);
        }
    }

    private static void DrawCustomPageFluent(ColumnDescriptor col, PageSettings s)
    {
        var borderColor = QuestPDF.Infrastructure.Color.FromHex($"{s.Style.BorderColor.R:X2}{s.Style.BorderColor.G:X2}{s.Style.BorderColor.B:X2}");
        var guideColor = QuestPDF.Infrastructure.Color.FromHex($"{s.Style.GuideColor.R:X2}{s.Style.GuideColor.G:X2}{s.Style.GuideColor.B:X2}");

        col.Item().ExtendVertical();

        // 1. Cover Image
        var mainImage = s.Elements.OfType<CustomImageElement>().FirstOrDefault();
        if (mainImage != null && !string.IsNullOrEmpty(mainImage.Base64Image))
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(mainImage.Base64Image);
                col.Item().PaddingBottom(15, Unit.Point).AlignCenter().Width(150, Unit.Point).Image(imageBytes);
            }
            catch { /* Ignore invalid base64 */ }
        }

        // 2. Title
        if (!string.IsNullOrWhiteSpace(s.Title))
        {
            col.Item().PaddingBottom(10, Unit.Point).Text(s.Title)
                .FontSize(s.TitleFontSizePt)
                .Bold()
                .FontFamily("Microsoft YaHei")
                .FontColor(borderColor)
                .AlignCenter();
        }

        // 3. Subtitle
        if (!string.IsNullOrWhiteSpace(s.Subtitle))
        {
            col.Item().PaddingBottom(15, Unit.Point).Text(s.Subtitle)
                .FontSize(s.SubtitleFontSizePt)
                .Italic()
                .FontFamily("Segoe UI")
                .FontColor(borderColor)
                .AlignCenter();
        }

        // 4. Custom Text Elements
        var texts = s.Elements.OfType<CustomTextElement>().ToList();
        foreach (var txt in texts)
        {
            if (string.IsNullOrWhiteSpace(txt.Text)) continue;

            var item = col.Item()
                .PaddingTop(txt.Y_Mm, Unit.Millimetre)
                .PaddingBottom(5, Unit.Millimetre);

            if (txt.Alignment == System.Drawing.StringAlignment.Center)
                item = item.AlignCenter();
            else if (txt.Alignment == System.Drawing.StringAlignment.Far)
                item = item.AlignRight();
            else
                item = item.AlignLeft();

            var textSpan = item.Text(txt.Text)
                .FontSize(txt.FontSizePt)
                .FontFamily(txt.FontFamily)
                .FontColor(QuestPDF.Infrastructure.Color.FromHex($"{txt.Color.R:X2}{txt.Color.G:X2}{txt.Color.B:X2}"));

            if (txt.IsBold) textSpan.Bold();
            if (txt.IsItalic) textSpan.Italic();
        }

        // 5. Divider Line (if author or notes exist)
        if (!string.IsNullOrWhiteSpace(s.AuthorName) || !string.IsNullOrWhiteSpace(s.Notes))
        {
            col.Item().PaddingVertical(15, Unit.Point)
                .PaddingHorizontal(50, Unit.Millimetre)
                .LineHorizontal(1).LineColor(guideColor);
        }

        // 6. Author Name
        if (!string.IsNullOrWhiteSpace(s.AuthorName))
        {
            col.Item().PaddingBottom(5, Unit.Point).Text($"姓名: {s.AuthorName}")
                .FontSize(s.AuthorFontSizePt)
                .FontFamily("Microsoft YaHei")
                .FontColor(borderColor)
                .AlignCenter();
        }

        // 7. Notes
        if (!string.IsNullOrWhiteSpace(s.Notes))
        {
            col.Item().PaddingBottom(10, Unit.Point).Text(s.Notes)
                .FontSize(s.NotesFontSizePt)
                .FontFamily("Segoe UI")
                .FontColor(borderColor)
                .AlignCenter();
        }

        col.Item().ExtendVertical();
    }

    private static void DrawLinedFluent(SKCanvas canvas, PageSettings s, SKSize space)
    {
        const float mmToPt = 72f / 25.4f;
        float lineSpacingPt = s.LineHeightMm * mmToPt;
        if (lineSpacingPt <= 0) return;

        using var linePen = new SKPaint { Color = new SKColor(s.LineColor.R, s.LineColor.G, s.LineColor.B), Style = SKPaintStyle.Stroke, StrokeWidth = s.LineWidthPt, IsAntialias = true };

        int lines = (int)(space.Height / lineSpacingPt);
        for (int i = 1; i <= lines; i++)
        {
            float y = i * lineSpacingPt;
            canvas.DrawLine(0, y, space.Width, y, linePen);
        }

        if (s.ShowMarginLine)
        {
            float marginX = 15f * mmToPt;
            using var marginPen = new SKPaint { Color = new SKColor(220, 150, 150), Style = SKPaintStyle.Stroke, StrokeWidth = 0.5f, IsAntialias = true };
            canvas.DrawLine(marginX, 0, marginX, space.Height, marginPen);
        }
    }
}
