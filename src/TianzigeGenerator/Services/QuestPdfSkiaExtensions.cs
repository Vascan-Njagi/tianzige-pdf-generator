using System;
using System.IO;
using System.Text;
using QuestPDF.Fluent;
using SkiaSharp;

namespace TianzigeGenerator.Services;

public static class QuestPdfSkiaExtensions
{
    public static void SkiaSharpSvgCanvas(this QuestPDF.Infrastructure.IContainer container, Action<SKCanvas, SKSize> drawOnCanvas)
    {
        container.Svg(size =>
        {
            using var stream = new MemoryStream();
            using (var canvas = SKSvgCanvas.Create(new SKRect(0, 0, size.Width, size.Height), stream))
            {
                drawOnCanvas(canvas, new SKSize(size.Width, size.Height));
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        });
    }
}
