using System.Text.Json.Serialization;

namespace TianzigeGenerator.Models;

[JsonDerivedType(typeof(CustomTextElement), typeDiscriminator: "text")]
[JsonDerivedType(typeof(CustomImageElement), typeDiscriminator: "image")]
public abstract class CustomElement
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public float X_Mm { get; set; } = 20f;
    public float Y_Mm { get; set; } = 20f;
    public float Width_Mm { get; set; } = 50f;
    public float Height_Mm { get; set; } = 20f;

    public abstract CustomElement Clone();
}
