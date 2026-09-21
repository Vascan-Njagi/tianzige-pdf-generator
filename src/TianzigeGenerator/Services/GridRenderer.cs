using System.Drawing;
using System.Drawing.Drawing2D;
using TianzigeGenerator.Models;

namespace TianzigeGenerator.Services;

public static class GridRenderer
{
    public static void RenderPage(Graphics g, PageSettings page, RectangleF targetRect)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        float pageWidthMm = page.EffectiveWidthMm;
        float pageHeightMm = page.EffectiveHeightMm;
        float scaleX = targetRect.Width / pageWidthMm;
        float scaleY = targetRect.Height / pageHeightMm;
        float scale = Math.Min(scaleX, scaleY);

        float drawnW = pageWidthMm * scale;
        float drawnH = pageHeightMm * scale;
        float offsetX = targetRect.X + (targetRect.Width - drawnW) / 2;
        float offsetY = targetRect.Y + (targetRect.Height - drawnH) / 2;

        // Draw drop shadow
        using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
        {
            var shadowRect = new RectangleF(offsetX + 4, offsetY + 4, drawnW, drawnH);
            g.FillRectangle(shadowBrush, shadowRect);
        }

        // Draw page background (add slight cream tint if white for preview distinction)
        Color previewBg = page.Style.PageBackground;
        if (previewBg.R == 255 && previewBg.G == 255 && previewBg.B == 255)
        {
            previewBg = Color.FromArgb(253, 252, 246); // Slight cream
        }
        using (var bgBrush = new SolidBrush(previewBg))
            g.FillRectangle(bgBrush, offsetX, offsetY, drawnW, drawnH);

        // Draw page border
        using (var pageBorderPen = new Pen(Color.FromArgb(180, 180, 180), 1f))
            g.DrawRectangle(pageBorderPen, offsetX, offsetY, drawnW, drawnH);

        float contentX = offsetX + page.MarginLeftMm * scale;
        float contentY = offsetY + page.MarginTopMm * scale;
        float contentW = (pageWidthMm - page.MarginLeftMm - page.MarginRightMm) * scale;
        float contentH = (pageHeightMm - page.MarginTopMm - page.MarginBottomMm) * scale;

        if (contentW <= 0 || contentH <= 0) return;

        switch (page.Type)
        {
            case PageType.TianzigeGrid:
            case PageType.MizigeGrid:
            case PageType.JiugonggeGrid:
            case PageType.EssayGrid:
                RenderGridPage(g, page, contentX, contentY, contentW, contentH, scale);
                break;
            case PageType.Vocabulary:
                RenderVocabularyPage(g, page, contentX, contentY, contentW, contentH, scale);
                break;
            case PageType.Custom:
                RenderCustomPage(g, page, offsetX, offsetY, drawnW, drawnH, scale);
                break;
            case PageType.Lined:
                RenderLinedPage(g, page, contentX, contentY, contentW, contentH, scale);
                break;
        }
    }

    private static void RenderGridPage(Graphics g, PageSettings page, float cx, float cy, float cw, float ch, float scale)
    {
        float cellSizePx = page.CellSizeMm * scale;
        float colSpacingPx = page.ColumnSpacingMm * scale;
        float rowSpacingPx = page.LineSpacingMm * scale;
        if (cellSizePx <= 0) return;

        int cols, rows;
        if (page.GridColsOverride > 0)
            cols = page.GridColsOverride;
        else
            cols = Math.Max(1, (int)((cw + colSpacingPx) / (cellSizePx + colSpacingPx)));
        
        if (page.GridRowsOverride > 0)
            rows = page.GridRowsOverride;
        else
            rows = Math.Max(1, (int)((ch + rowSpacingPx) / (cellSizePx + rowSpacingPx)));

        float totalGridW = cols * cellSizePx + (cols - 1) * colSpacingPx;
        float totalGridH = rows * cellSizePx + (rows - 1) * rowSpacingPx;
        float startX = cx + (cw - totalGridW) / 2;
        float startY = cy + (ch - totalGridH) / 2;

        // Draw column headers if specified
        if (!string.IsNullOrEmpty(page.ColumnHeaders))
        {
            var headers = page.ColumnHeaders.Split(',');
            float headerFontSize = Math.Max(6, cellSizePx * 0.2f);
            using var headerFont = new Font("Microsoft YaHei", headerFontSize);
            using var headerBrush = new SolidBrush(Color.FromArgb(100, page.Style.BorderColor));
            using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far };
            
            for (int c = 0; c < Math.Min(cols, headers.Length); c++)
            {
                if (!string.IsNullOrWhiteSpace(headers[c]))
                {
                    float x = startX + c * (cellSizePx + colSpacingPx);
                    g.DrawString(headers[c].Trim(), headerFont, headerBrush, x + cellSizePx / 2, startY - 2, fmt);
                }
            }
        }

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                float x = startX + c * (cellSizePx + colSpacingPx);
                float y = startY + r * (cellSizePx + rowSpacingPx);
                RenderCell(g, page, new RectangleF(x, y, cellSizePx, cellSizePx));
            }
    }

    private static void RenderCell(Graphics g, PageSettings page, RectangleF cell)
    {
        // Draw cell shading if enabled
        if (page.Style.ShowCellShading)
        {
            using var shadingBrush = new SolidBrush(page.Style.CellShadingColor);
            if (page.Style.CornerRadiusPt > 0)
            {
                using var path = CreateRoundedRectPath(cell, page.Style.CornerRadiusPt);
                g.FillPath(shadingBrush, path);
            }
            else
            {
                g.FillRectangle(shadingBrush, cell);
            }
        }

        // Draw cell background
        using (var bgBrush = new SolidBrush(page.Style.CellBackground))
        {
            if (page.Style.CornerRadiusPt > 0)
            {
                using var path = CreateRoundedRectPath(cell, page.Style.CornerRadiusPt);
                g.FillPath(bgBrush, path);
            }
            else
            {
                g.FillRectangle(bgBrush, cell);
            }
        }

        // Draw cell border
        using (var borderPen = new Pen(page.Style.BorderColor, page.Style.BorderWidthPt))
        {
            borderPen.LineJoin = LineJoin.Miter;
            if (page.Style.CornerRadiusPt > 0)
            {
                using var path = CreateRoundedRectPath(cell, page.Style.CornerRadiusPt);
                g.DrawPath(borderPen, path);
            }
            else
            {
                g.DrawRectangle(borderPen, cell.X, cell.Y, cell.Width, cell.Height);
            }
        }

        // Apply inner padding for guide lines (skip for Essay Grid since it has no inner guides)
        if (page.Type == PageType.EssayGrid) return;

        float paddingPx = page.Style.CellInnerPaddingMm * (cell.Width / page.CellSizeMm);
        var guideRect = new RectangleF(
            cell.X + paddingPx,
            cell.Y + paddingPx,
            cell.Width - 2 * paddingPx,
            cell.Height - 2 * paddingPx
        );

        using (var guidePen = new Pen(page.Style.GuideColor, page.Style.GuideWidthPt))
        {
            if (page.Style.DottedGuides)
            {
                guidePen.DashPattern = new float[] { 0, 2 };
                guidePen.DashCap = DashCap.Round;
            }
            else if (page.Style.DashedGuides)
            {
                guidePen.DashPattern = page.Style.DashPattern;
            }
            float midX = guideRect.X + guideRect.Width / 2;
            float midY = guideRect.Y + guideRect.Height / 2;

            if (page.Type == PageType.TianzigeGrid)
            {
                g.DrawLine(guidePen, guideRect.X, midY, guideRect.Right, midY);
                g.DrawLine(guidePen, midX, guideRect.Y, midX, guideRect.Bottom);
            }
            if (page.Type == PageType.JiugonggeGrid)
            {
                float tw = guideRect.Width / 3, th = guideRect.Height / 3;
                g.DrawLine(guidePen, guideRect.X + tw, guideRect.Y, guideRect.X + tw, guideRect.Bottom);
                g.DrawLine(guidePen, guideRect.X + 2 * tw, guideRect.Y, guideRect.X + 2 * tw, guideRect.Bottom);
                g.DrawLine(guidePen, guideRect.X, guideRect.Y + th, guideRect.Right, guideRect.Y + th);
                g.DrawLine(guidePen, guideRect.X, guideRect.Y + 2 * th, guideRect.Right, guideRect.Y + 2 * th);
            }
        }
    }

    private static GraphicsPath CreateRoundedRectPath(RectangleF rect, float radius)
    {
        var path = new GraphicsPath();
        float d = radius * 2;
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static void RenderVocabularyPage(Graphics g, PageSettings page, float cx, float cy, float cw, float ch, float scale)
    {
        float cellSizePx = page.CellSizeMm * scale;
        float pinyinHeightPx = page.PinyinHeightMm * scale;
        float colSpacingPx = page.ColumnSpacingMm * scale;
        float rowSpacingPx = page.LineSpacingMm * scale;
        if (cellSizePx <= 0) return;

        // In vocabulary mode, we render pairs of (Pinyin Box + Character Box) vertically,
        // with translation/meaning lines to the right of the grid block.

        int cols, rows;
        if (page.GridColsOverride > 0)
            cols = page.GridColsOverride;
        else
            cols = Math.Max(1, (int)(cw * 0.4f / (cellSizePx + colSpacingPx))); // Use ~40% of width for grids

        if (page.GridRowsOverride > 0)
            rows = page.GridRowsOverride;
        else
            rows = Math.Max(1, (int)((ch + rowSpacingPx) / (cellSizePx + pinyinHeightPx + rowSpacingPx)));

        float totalGridW = cols * cellSizePx + (cols - 1) * colSpacingPx;
        float totalGridH = rows * (cellSizePx + pinyinHeightPx) + (rows - 1) * rowSpacingPx;
        float startX = cx; // Align left
        float startY = cy + (ch - totalGridH) / 2; // Center vertically

        using var borderPen = new Pen(page.Style.BorderColor, page.Style.BorderWidthPt);
        using var linePen = new Pen(page.LineColor, page.LineWidthPt);

        for (int r = 0; r < rows; r++)
        {
            float rowY = startY + r * (cellSizePx + pinyinHeightPx + rowSpacingPx);

            // Draw grids for this row
            for (int c = 0; c < cols; c++)
            {
                float x = startX + c * (cellSizePx + colSpacingPx);

                // Draw Pinyin Box
                g.DrawRectangle(borderPen, x, rowY, cellSizePx, pinyinHeightPx);

                // Draw Character Box
                var cellRect = new RectangleF(x, rowY + pinyinHeightPx, cellSizePx, cellSizePx);
                RenderCell(g, page, cellRect);
            }

            // Draw translation/meaning lines to the right of the grids
            float linesStartX = startX + totalGridW + (10 * scale);
            float linesW = cw - totalGridW - (10 * scale);

            float line1Y = rowY + pinyinHeightPx + (cellSizePx * 0.33f);
            float line2Y = rowY + pinyinHeightPx + (cellSizePx * 0.66f);

            g.DrawLine(linePen, linesStartX, line1Y, linesStartX + linesW, line1Y);
            g.DrawLine(linePen, linesStartX, line2Y, linesStartX + linesW, line2Y);
        }
    }

    private static void RenderCustomPage(Graphics g, PageSettings page, float px, float py, float pw, float ph, float scale)
    {
        // Draw page decorative borders
        float inset = 15 * scale;
        var borderRect = new RectangleF(px + inset, py + inset, pw - 2 * inset, ph - 2 * inset);
        using (var borderPen = new Pen(page.Style.BorderColor, 2f))
            g.DrawRectangle(borderPen, borderRect.X, borderRect.Y, borderRect.Width, borderRect.Height);

        float innerInset = 3 * scale;
        var innerRect = new RectangleF(borderRect.X + innerInset, borderRect.Y + innerInset,
            borderRect.Width - 2 * innerInset, borderRect.Height - 2 * innerInset);
        using (var innerPen = new Pen(page.Style.BorderColor, 0.5f))
            g.DrawRectangle(innerPen, innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height);

        // Content bounds (respecting page margins or inner border inset)
        float leftMargin = Math.Max(page.MarginLeftMm * scale, inset + 10 * scale);
        float rightMargin = Math.Max(page.MarginRightMm * scale, inset + 10 * scale);
        float topMargin = Math.Max(page.MarginTopMm * scale, inset + 10 * scale);
        float bottomMargin = Math.Max(page.MarginBottomMm * scale, inset + 10 * scale);

        float contentX = px + leftMargin;
        float contentW = pw - leftMargin - rightMargin;
        float contentH = ph - topMargin - bottomMargin;

        if (contentW <= 0 || contentH <= 0) return;

        var images = page.Elements.OfType<CustomImageElement>().ToList();
        var texts = page.Elements.OfType<CustomTextElement>().ToList();

        // Measure total height of all elements to center content vertically on the page
        float totalContentH = 0f;

        // Image measurement
        var mainImage = images.FirstOrDefault();
        Bitmap? loadedImg = null;
        float imgDrawW = 0f, imgDrawH = 0f;
        if (mainImage != null && !string.IsNullOrEmpty(mainImage.Base64Image))
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(mainImage.Base64Image);
                using var ms = new MemoryStream(imageBytes);
                using var tempImg = Image.FromStream(ms);
                loadedImg = new Bitmap(tempImg);

                float maxImgSize = 60 * scale;
                float imgAspect = (float)loadedImg.Width / loadedImg.Height;
                if (imgAspect >= 1)
                {
                    imgDrawW = Math.Min(contentW, maxImgSize);
                    imgDrawH = imgDrawW / imgAspect;
                }
                else
                {
                    imgDrawH = Math.Min(contentH * 0.4f, maxImgSize);
                    imgDrawW = imgDrawH * imgAspect;
                }
                totalContentH += imgDrawH + 15 * scale;
            }
            catch
            {
                loadedImg?.Dispose();
                loadedImg = null;
            }
        }

        Font? titleFont = null;
        Font? subFont = null;
        Font? authorFont = null;
        Font? notesFont = null;
        var textFonts = new List<(CustomTextElement element, Font font, SizeF size)>();

        try
        {
            // Title measurement
            SizeF titleSize = SizeF.Empty;
            if (!string.IsNullOrWhiteSpace(page.Title))
            {
                float fSize = Math.Max(8, page.TitleFontSizePt * scale * 0.35f);
                titleFont = new Font("Microsoft YaHei", fSize, FontStyle.Bold);
                titleSize = g.MeasureString(page.Title, titleFont, (int)contentW);
                totalContentH += titleSize.Height + 10 * scale;
            }

            // Subtitle measurement
            SizeF subSize = SizeF.Empty;
            if (!string.IsNullOrWhiteSpace(page.Subtitle))
            {
                float fSize = Math.Max(6, page.SubtitleFontSizePt * scale * 0.35f);
                subFont = new Font("Segoe UI", fSize, FontStyle.Italic);
                subSize = g.MeasureString(page.Subtitle, subFont, (int)contentW);
                totalContentH += subSize.Height + 15 * scale;
            }

            // Custom text blocks measurement
            foreach (var txt in texts)
            {
                if (string.IsNullOrWhiteSpace(txt.Text)) continue;
                float fSize = Math.Max(6, txt.FontSizePt * scale * 0.35f);
                FontStyle style = FontStyle.Regular;
                if (txt.IsBold) style |= FontStyle.Bold;
                if (txt.IsItalic) style |= FontStyle.Italic;

                var font = new Font(txt.FontFamily, fSize, style);
                var size = g.MeasureString(txt.Text, font, (int)contentW);
                textFonts.Add((txt, font, size));

                totalContentH += (txt.Y_Mm * scale) + size.Height + 5 * scale;
            }

            // Divider line measurement
            bool showDivider = !string.IsNullOrWhiteSpace(page.AuthorName) || !string.IsNullOrWhiteSpace(page.Notes);
            if (showDivider)
            {
                totalContentH += 25 * scale;
            }

            // Author measurement
            SizeF authorSize = SizeF.Empty;
            string authorStr = !string.IsNullOrWhiteSpace(page.AuthorName) ? $"姓名: {page.AuthorName}" : "";
            if (!string.IsNullOrEmpty(authorStr))
            {
                float fSize = Math.Max(6, page.AuthorFontSizePt * scale * 0.35f);
                authorFont = new Font("Microsoft YaHei", fSize, FontStyle.Regular);
                authorSize = g.MeasureString(authorStr, authorFont, (int)contentW);
                totalContentH += authorSize.Height + 10 * scale;
            }

            // Notes measurement
            SizeF notesSize = SizeF.Empty;
            if (!string.IsNullOrWhiteSpace(page.Notes))
            {
                float fSize = Math.Max(6, page.NotesFontSizePt * scale * 0.35f);
                notesFont = new Font("Segoe UI", fSize, FontStyle.Regular);
                notesSize = g.MeasureString(page.Notes, notesFont, (int)contentW);
                totalContentH += notesSize.Height + 10 * scale;
            }

            // Determine start Y (center vertically if total content fits in available content area)
            float startY = py + topMargin;
            if (totalContentH < contentH)
            {
                startY += (contentH - totalContentH) / 2f;
            }

            float currentY = startY;

            // Render Cover Image
            if (loadedImg != null)
            {
                g.DrawImage(loadedImg, px + pw / 2 - imgDrawW / 2, currentY, imgDrawW, imgDrawH);
                currentY += imgDrawH + 15 * scale;
            }

            // Render Title
            if (titleFont != null)
            {
                using var brush = new SolidBrush(page.Style.BorderColor);
                using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                var rect = new RectangleF(contentX, currentY, contentW, titleSize.Height + 5 * scale);
                g.DrawString(page.Title, titleFont, brush, rect, fmt);
                currentY += titleSize.Height + 10 * scale;
            }

            // Render Subtitle
            if (subFont != null)
            {
                using var brush = new SolidBrush(page.Style.BorderColor);
                using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                var rect = new RectangleF(contentX, currentY, contentW, subSize.Height + 5 * scale);
                g.DrawString(page.Subtitle, subFont, brush, rect, fmt);
                currentY += subSize.Height + 15 * scale;
            }

            // Render Custom Text Elements
            foreach (var (txt, font, size) in textFonts)
            {
                currentY += txt.Y_Mm * scale;

                using var brush = new SolidBrush(txt.Color);
                using var fmt = new StringFormat
                {
                    LineAlignment = StringAlignment.Near,
                    Alignment = txt.Alignment switch
                    {
                        StringAlignment.Near => StringAlignment.Near,
                        StringAlignment.Far => StringAlignment.Far,
                        _ => StringAlignment.Center
                    }
                };

                var rect = new RectangleF(contentX, currentY, contentW, size.Height + 5 * scale);
                g.DrawString(txt.Text, font, brush, rect, fmt);
                currentY += size.Height + 5 * scale;
            }

            // Render Divider Line
            if (showDivider)
            {
                float lineW = Math.Min(contentW * 0.6f, 100 * scale);
                using var guidePen = new Pen(page.Style.GuideColor, 1f);
                g.DrawLine(guidePen, px + pw / 2 - lineW / 2, currentY + 10 * scale, px + pw / 2 + lineW / 2, currentY + 10 * scale);
                currentY += 25 * scale;
            }

            // Render Author
            if (authorFont != null)
            {
                using var brush = new SolidBrush(page.Style.BorderColor);
                using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                var rect = new RectangleF(contentX, currentY, contentW, authorSize.Height + 5 * scale);
                g.DrawString(authorStr, authorFont, brush, rect, fmt);
                currentY += authorSize.Height + 10 * scale;
            }

            // Render Notes
            if (notesFont != null)
            {
                using var brush = new SolidBrush(page.Style.BorderColor);
                using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                var rect = new RectangleF(contentX, currentY, contentW, notesSize.Height + 5 * scale);
                g.DrawString(page.Notes, notesFont, brush, rect, fmt);
                currentY += notesSize.Height + 10 * scale;
            }
        }
        finally
        {
            loadedImg?.Dispose();
            titleFont?.Dispose();
            subFont?.Dispose();
            authorFont?.Dispose();
            notesFont?.Dispose();
            foreach (var (_, font, _) in textFonts)
            {
                font.Dispose();
            }
        }
    }

    private static void RenderLinedPage(Graphics g, PageSettings page, float cx, float cy, float cw, float ch, float scale)
    {
        float lineSpacingPx = page.LineHeightMm * scale;
        if (lineSpacingPx <= 0) return;

        int lines = (int)(ch / lineSpacingPx);
        using (var linePen = new Pen(page.LineColor, page.LineWidthPt))
            for (int i = 1; i <= lines; i++)
            {
                float y = cy + i * lineSpacingPx;
                g.DrawLine(linePen, cx, y, cx + cw, y);
            }

        if (page.ShowMarginLine)
        {
            float marginX = cx + 15 * scale;
            using (var marginPen = new Pen(Color.FromArgb(220, 150, 150), 0.5f))
                g.DrawLine(marginPen, marginX, cy, marginX, cy + ch);
        }
    }
}