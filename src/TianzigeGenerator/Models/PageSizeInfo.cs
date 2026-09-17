namespace TianzigeGenerator.Models;

/// <summary>
/// Standard page sizes with dimensions in millimeters.
/// </summary>
public record PageSizeInfo(string Name, float WidthMm, float HeightMm)
{
    public static readonly PageSizeInfo A3 = new("A3", 297, 420);
    public static readonly PageSizeInfo A4 = new("A4", 210, 297);
    public static readonly PageSizeInfo A5 = new("A5", 148, 210);
    public static readonly PageSizeInfo B4 = new("B4 (JIS)", 257, 364);
    public static readonly PageSizeInfo B5 = new("B5 (JIS)", 182, 257);
    public static readonly PageSizeInfo Letter = new("Letter", 215.9f, 279.4f);
    public static readonly PageSizeInfo Legal = new("Legal", 215.9f, 355.6f);
    public static readonly PageSizeInfo Tabloid = new("Tabloid (11x17)", 279.4f, 431.8f);

    public static IReadOnlyList<PageSizeInfo> All { get; } = new[]
    {
        A3, A4, A5, B4, B5, Letter, Legal, Tabloid
    };

    public static PageSizeInfo Custom(float w, float h, string name = "Custom") => new(name, w, h);

    public bool IsLandscape => WidthMm > HeightMm;
}
