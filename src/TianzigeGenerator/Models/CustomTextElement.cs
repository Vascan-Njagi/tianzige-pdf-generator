using System.Drawing;

namespace TianzigeGenerator.Models;

public class CustomTextElement : CustomElement
{
    public string Text { get; set; } = "Double-click to edit";
    public float FontSizePt { get; set; } = 14f;
    public string FontFamily { get; set; } = "Microsoft YaHei";
    public bool IsBold { get; set; } = false;
    public bool IsItalic { get; set; } = false;
    public Color Color { get; set; } = Color.Black;
    public StringAlignment Alignment { get; set; } = StringAlignment.Near;

    public override CustomElement Clone() => new CustomTextElement
    {
        Id = Guid.NewGuid().ToString("N")[..8],
        X_Mm = X_Mm,
        Y_Mm = Y_Mm,
        Width_Mm = Width_Mm,
        Height_Mm = Height_Mm,
        Text = Text,
        FontSizePt = FontSizePt,
        FontFamily = FontFamily,
        IsBold = IsBold,
        IsItalic = IsItalic,
        Color = Color,
        Alignment = Alignment
    };
}
