using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TianzigeGenerator.Models;
using TianzigeGenerator.Services;

namespace TianzigeGenerator.Controls;

public class GridPreviewControl : Control
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PageSettings? PageSettings { get; set; }

    public GridPreviewControl()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        BackColor = Color.FromArgb(240, 240, 245);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (PageSettings == null) return;

        var g = e.Graphics;
        g.Clear(BackColor);

        // Add more padding to ensure the page is not clipped
        var padding = 20f;
        var rect = new RectangleF(padding, padding, Width - 2 * padding, Height - 2 * padding);

        // Add drop shadow effect
        using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
        {
            var shadowRect = new RectangleF(rect.X + 3, rect.Y + 3, rect.Width, rect.Height);
            g.FillRectangle(shadowBrush, shadowRect);
        }

        GridRenderer.RenderPage(g, PageSettings, rect);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        Invalidate();
    }
}
