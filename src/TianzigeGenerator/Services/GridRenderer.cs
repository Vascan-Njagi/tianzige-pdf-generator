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

        using (var bgBrush = new SolidBrush(page.Style.PageBackground))
            g.FillRectangle(bgBrush, targetRect);

        using (var pageBorderPen = new Pen(Color.FromArgb(180, 180, 180), 1f))
            g.DrawRectangle(pageBorderPen, targetRect.X, targetRect.Y, targetRect.Width, targetRect.Height);

        float pageWidthMm = page.EffectiveWidthMm;
        float pageHeightMm = page.EffectiveHeightMm;
        float scaleX = targetRect.Width / pageWidthMm;
        float scaleY = targetRect.Height / pageHeightMm;
        float scale = Math.Min(scaleX, scaleY);

        float drawnW = pageWidthMm * scale;
        float drawnH = pageHeightMm * scale;
        float offsetX = targetRect.X + (targetRect.Width - drawnW) / 2;
        float offsetY = targetRect.Y + (targetRect.Height - drawnH) / 2;

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
                RenderGridPage(g, page, contentX, contentY, contentW, contentH, scale);
                break;
            case PageType.Cover:
                RenderCoverPage(g, page, offsetX, offsetY, drawnW, drawnH, scale);
                break;
            case PageType.Ending:
                RenderEndingPage(g, page, offsetX, offsetY, drawnW, drawnH, scale);
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
            var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far };
            
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

        // Apply inner padding for guide lines
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

            if (page.Type == PageType.TianzigeGrid || page.Type == PageType.MizigeGrid)
            {
                g.DrawLine(guidePen, guideRect.X, midY, guideRect.Right, midY);
                g.DrawLine(guidePen, midX, guideRect.Y, midX, guideRect.Bottom);
            }
            if (page.Type == PageType.MizigeGrid)
            {
                g.DrawLine(guidePen, guideRect.X, guideRect.Y, guideRect.Right, guideRect.Bottom);
                g.DrawLine(guidePen, guideRect.Right, guideRect.Y, guideRect.X, guideRect.Bottom);
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

    private static void RenderCoverPage(Graphics g, PageSettings page, float px, float py, float pw, float ph, float scale)
    {
        float inset = 15 * scale;
        var borderRect = new RectangleF(px + inset, py + inset, pw - 2 * inset, ph - 2 * inset);
        using (var borderPen = new Pen(page.Style.BorderColor, 2f))
            g.DrawRectangle(borderPen, borderRect.X, borderRect.Y, borderRect.Width, borderRect.Height);

        float innerInset = 3 * scale;
        var innerRect = new RectangleF(borderRect.X + innerInset, borderRect.Y + innerInset,
            borderRect.Width - 2 * innerInset, borderRect.Height - 2 * innerInset);
        using (var innerPen = new Pen(page.Style.BorderColor, 0.5f))
            g.DrawRectangle(innerPen, innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height);

        float titleFontSize = Math.Max(12, page.TitleFontSizePt * scale * 0.35f);
        using (var titleFont = new Font("Microsoft YaHei", titleFontSize, FontStyle.Bold))
        using (var titleBrush = new SolidBrush(page.Style.BorderColor))
        {
            var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(page.Title, titleFont, titleBrush, px + pw / 2, py + ph * 0.35f, fmt);
        }

        if (!string.IsNullOrEmpty(page.Subtitle))
        {
            float subFontSize = Math.Max(8, page.SubtitleFontSizePt * scale * 0.35f);
            using var subFont = new Font("Segoe UI", subFontSize, FontStyle.Italic);
            using var subBrush = new SolidBrush(Color.FromArgb(150, page.Style.BorderColor));
            var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(page.Subtitle, subFont, subBrush, px + pw / 2, py + ph * 0.45f, fmt);
        }

        float divY = py + ph * 0.55f;
        float divW = pw * 0.3f;
        using (var divPen = new Pen(page.Style.GuideColor, 1f))
            g.DrawLine(divPen, px + pw / 2 - divW / 2, divY, px + pw / 2 + divW / 2, divY);

        if (!string.IsNullOrEmpty(page.AuthorName))
        {
            float nameFontSize = Math.Max(8, page.AuthorFontSizePt * scale * 0.35f);
            using var nameFont = new Font("Segoe UI", nameFontSize);
            using var nameBrush = new SolidBrush(Color.FromArgb(100, page.Style.BorderColor));
            var fmt = new StringFormat { Alignment = StringAlignment.Center };
            g.DrawString($"姓名: {page.AuthorName}", nameFont, nameBrush, px + pw / 2, py + ph * 0.65f, fmt);
        }

        if (!string.IsNullOrEmpty(page.Notes))
        {
            float notesFontSize = Math.Max(7, page.NotesFontSizePt * scale * 0.35f);
            using var notesFont = new Font("Segoe UI", notesFontSize);
            using var notesBrush = new SolidBrush(Color.FromArgb(80, page.Style.BorderColor));
            var fmt = new StringFormat { Alignment = StringAlignment.Center };
            g.DrawString(page.Notes, notesFont, notesBrush, px + pw / 2, py + ph * 0.75f, fmt);
        }
    }

    private static void RenderEndingPage(Graphics g, PageSettings page, float px, float py, float pw, float ph, float scale)
    {
        float inset = 20 * scale;
        var borderRect = new RectangleF(px + inset, py + inset, pw - 2 * inset, ph - 2 * inset);
        using (var borderPen = new Pen(page.Style.BorderColor, 1.5f))
            g.DrawRectangle(borderPen, borderRect.X, borderRect.Y, borderRect.Width, borderRect.Height);

        float titleFontSize = Math.Max(12, page.TitleFontSizePt * scale * 0.35f);
        using (var titleFont = new Font("Microsoft YaHei", titleFontSize, FontStyle.Bold))
        using (var titleBrush = new SolidBrush(page.Style.BorderColor))
        {
            var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(page.Title, titleFont, titleBrush, px + pw / 2, py + ph * 0.4f, fmt);
        }

        if (!string.IsNullOrEmpty(page.Notes))
        {
            float notesFontSize = Math.Max(8, page.NotesFontSizePt * scale * 0.35f);
            using var notesFont = new Font("Segoe UI", notesFontSize);
            using var notesBrush = new SolidBrush(Color.FromArgb(120, page.Style.BorderColor));
            var fmt = new StringFormat { Alignment = StringAlignment.Center };
            g.DrawString(page.Notes, notesFont, notesBrush, px + pw / 2, py + ph * 0.55f, fmt);
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