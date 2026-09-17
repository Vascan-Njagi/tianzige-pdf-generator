using System.Drawing;

namespace TianzigeGenerator.Models;

/// <summary>
/// Visual style settings for grid rendering.
/// </summary>
public class GridStyle
{
    /// <summary>Color of the outer solid border.</summary>
    public Color BorderColor { get; set; } = Color.FromArgb(40, 40, 40);

    /// <summary>Width of the outer border in points.</summary>
    public float BorderWidthPt { get; set; } = 1.5f;

    /// <summary>Color of the inner cross / guide lines.</summary>
    public Color GuideColor { get; set; } = Color.FromArgb(180, 60, 60);

    /// <summary>Width of inner guide lines in points.</summary>
    public float GuideWidthPt { get; set; } = 0.6f;

    /// <summary>Whether to draw dashed guide lines (true) or solid (false).</summary>
    public bool DashedGuides { get; set; } = true;

    /// <summary>Whether to draw dotted guide lines (true) instead of dashed.</summary>
    public bool DottedGuides { get; set; } = false;

    /// <summary>Whether to draw diagonal lines (for Mizige).</summary>
    public bool DrawDiagonals { get; set; } = false;

    /// <summary>Dash pattern for guide lines (in points).</summary>
    public float[] DashPattern { get; set; } = new[] { 3f, 2f };

    /// <summary>Background color of the grid cells.</summary>
    public Color CellBackground { get; set; } = Color.White;

    /// <summary>Background color of the page.</summary>
    public Color PageBackground { get; set; } = Color.White;

    /// <summary>Corner radius for cells in points (0 = square corners).</summary>
    public float CornerRadiusPt { get; set; } = 0f;

    /// <summary>Inner padding inside each cell in mm (space between border and guides).</summary>
    public float CellInnerPaddingMm { get; set; } = 0f;

    /// <summary>Whether to show a light grid background behind cells.</summary>
    public bool ShowCellShading { get; set; } = false;

    /// <summary>Shading color for cells (used when ShowCellShading is true).</summary>
    public Color CellShadingColor { get; set; } = Color.FromArgb(250, 248, 240);

    public GridStyle Clone() => new GridStyle
    {
        BorderColor = BorderColor,
        BorderWidthPt = BorderWidthPt,
        GuideColor = GuideColor,
        GuideWidthPt = GuideWidthPt,
        DashedGuides = DashedGuides,
        DottedGuides = DottedGuides,
        DrawDiagonals = DrawDiagonals,
        DashPattern = (float[])DashPattern.Clone(),
        CellBackground = CellBackground,
        PageBackground = PageBackground,
        CornerRadiusPt = CornerRadiusPt,
        CellInnerPaddingMm = CellInnerPaddingMm,
        ShowCellShading = ShowCellShading,
        CellShadingColor = CellShadingColor
    };
}
