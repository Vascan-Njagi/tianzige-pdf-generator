namespace TianzigeGenerator.Models;

/// <summary>
/// Settings for a single page in the document.
/// </summary>
public class PageSettings
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public PageType Type { get; set; } = PageType.TianzigeGrid;

    // Page dimensions
    public string PageSizeName { get; set; } = "A4";
    public float PageWidthMm { get; set; } = 210f;
    public float PageHeightMm { get; set; } = 297f;
    public bool Landscape { get; set; } = false;

    // Global settings
    public bool ShowPageNumbers { get; set; } = false;

    // Margins (mm)
    public float MarginTopMm { get; set; } = 15f;
    public float MarginBottomMm { get; set; } = 15f;
    public float MarginLeftMm { get; set; } = 15f;
    public float MarginRightMm { get; set; } = 15f;

    // Elements for WYSIWYG
    public List<CustomElement> Elements { get; set; } = new();

    // Grid cell settings
    public float CellSizeMm { get; set; } = 15f;       // Size of each square cell
    public float CellSpacingMm { get; set; } = 0f;     // Gap between cells
    public float LineSpacingMm { get; set; } = 8f;     // Vertical spacing between rows of cells
    public float ColumnSpacingMm { get; set; } = 8f;   // Horizontal spacing between columns of cells
    public float PinyinHeightMm { get; set; } = 6f;    // Pinyin box height for Vocabulary pages

    // Grid style
    public GridStyle Style { get; set; } = new();

    // Standard Fallback Content Fields
    public string Title { get; set; } = "汉字书写练习";
    public string Subtitle { get; set; } = "Chinese Character Writing Practice";
    public string AuthorName { get; set; } = "";
    public string Notes { get; set; } = "";

    // Cover/Ending page font sizes (in points)
    public float TitleFontSizePt { get; set; } = 36f;
    public float SubtitleFontSizePt { get; set; } = 14f;
    public float AuthorFontSizePt { get; set; } = 14f;
    public float NotesFontSizePt { get; set; } = 10f;

    // Lined page settings
    public float LineHeightMm { get; set; } = 8f;
    public Color LineColor { get; set; } = System.Drawing.Color.FromArgb(180, 200, 220);
    public float LineWidthPt { get; set; } = 0.5f;

    // Custompage size (used when PageSizeName is "Custom")
    public float CustomWidthMm { get; set; } = 210f;
    public float CustomHeightMm { get; set; } = 297f;

    // Grid column headers (text shown above each column of cells)
    public string ColumnHeaders { get; set; } = "";

    // User-defined page label (shown in page list instead of auto-generated)
    public string PageLabel { get; set; } = "";

    // Show margin line (red vertical line on lined pages)
    public bool ShowMarginLine { get; set; } = true;

    // Number of grid rows/cols override (0 = auto)
    public int GridRowsOverride { get; set; } = 0;
    public int GridColsOverride { get; set; } = 0;

    // Display label for page list
    public string DisplayName => !string.IsNullOrEmpty(PageLabel) ? PageLabel : (Type switch
    {
        PageType.TianzigeGrid => $"田 {CellSizeMm}mm Tianzige",
        PageType.MizigeGrid => $"米 {CellSizeMm}mm Mizige",
        PageType.JiugonggeGrid => $"九 {CellSizeMm}mm Jiugongge",
        PageType.EssayGrid => $"作 {CellSizeMm}mm Essay Grid",
        PageType.Vocabulary => $"字 {CellSizeMm}mm Vocabulary",
        PageType.Blank => "▢ Blank Page",
        PageType.Lined => $"≡ Lined ({LineHeightMm}mm)",
        PageType.Custom => "📝 Custom Page",
        _ => Type.ToString()
    });

    private static string Truncate(string s, int max) =>
        string.IsNullOrEmpty(s) ? "" : s.Length <= max ? s : s[..max] + "…";

    public float EffectiveWidthMm => Landscape ? PageHeightMm : PageWidthMm;
    public float EffectiveHeightMm => Landscape ? PageWidthMm : PageHeightMm;

    public PageSettings Clone()
    {
        return new PageSettings
        {
            Type = Type,
            PageSizeName = PageSizeName,
            PageWidthMm = PageWidthMm,
            PageHeightMm = PageHeightMm,
            Landscape = Landscape,
            ShowPageNumbers = ShowPageNumbers,
            Elements = Elements.Select(e => e.Clone()).ToList(),
            MarginTopMm = MarginTopMm,
            MarginBottomMm = MarginBottomMm,
            MarginLeftMm = MarginLeftMm,
            MarginRightMm = MarginRightMm,
            CellSizeMm = CellSizeMm,
            CellSpacingMm = CellSpacingMm,
            LineSpacingMm = LineSpacingMm,
            ColumnSpacingMm = ColumnSpacingMm,
            PinyinHeightMm = PinyinHeightMm,
            Style = Style.Clone(),
            Title = Title,
            Subtitle = Subtitle,
            AuthorName = AuthorName,
            Notes = Notes,
            LineHeightMm = LineHeightMm,
            LineColor = LineColor,
            LineWidthPt = LineWidthPt,
            TitleFontSizePt = TitleFontSizePt,
            SubtitleFontSizePt = SubtitleFontSizePt,
            AuthorFontSizePt = AuthorFontSizePt,
            NotesFontSizePt = NotesFontSizePt,
            CustomWidthMm = CustomWidthMm,
            CustomHeightMm = CustomHeightMm,
            ColumnHeaders = ColumnHeaders,
            PageLabel = PageLabel,
            ShowMarginLine = ShowMarginLine,
            GridRowsOverride = GridRowsOverride,
            GridColsOverride = GridColsOverride
        };
    }
}
