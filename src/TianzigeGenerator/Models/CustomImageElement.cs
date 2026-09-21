namespace TianzigeGenerator.Models;

public class CustomImageElement : CustomElement
{
    public string Base64Image { get; set; } = "";

    public override CustomElement Clone() => new CustomImageElement
    {
        Id = Guid.NewGuid().ToString("N")[..8],
        X_Mm = X_Mm,
        Y_Mm = Y_Mm,
        Width_Mm = Width_Mm,
        Height_Mm = Height_Mm,
        Base64Image = Base64Image
    };
}
