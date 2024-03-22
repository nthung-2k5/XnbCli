using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Serilog;
using StbImageWriteSharp;
using XnbReader.FileFormat;
using XnbReader.MonoGameShims;
using XnbReader.Texture;

namespace XnbCli;

public static class ExportAction
{
    private static readonly ImageWriter ImgWriter = new();

    public static void ExportFile(string filename, XnbFile xnb, JsonTypeInfo<XnbFile> context)
    {
        if (xnb.Content is null)
        {
            return;
        }
        
        string dirPath = Path.GetDirectoryName(filename)!;

        if (!string.IsNullOrEmpty(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        object temp = xnb.Content;
        ExportExternalResource(filename, ref temp);
        xnb.Content = temp;

        using var json = File.Create(filename);
        using var utf8Json = new Utf8JsonWriter(json, new JsonWriterOptions { Indented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

        JsonSerializer.Serialize(utf8Json, xnb, context);

        if (xnb.Content is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private static void ExportExternalResource(string filename, ref object content)
    {
        if (content is not (Texture2D or SpriteFont or Effect or TBin or BmFont))
        {
            return;
        }
        // log that we are exporting additional data
        Log.Information("Exporting {content} ...", content.GetType().Name);

        // switch over possible export types
        string extension = content switch
        {
            Texture2D => "png",
            SpriteFont => "png",
            Effect => "cso",
            TBin => "tbin",
            BmFont => "xml",
            _ => throw new ArgumentOutOfRangeException(nameof(content), content, null)
        };

        string outputFilename = Path.ChangeExtension(filename, extension);
        var resourceFile = new FileInfo(outputFilename);
        using var resourceWriter = resourceFile.CreateText();

        switch (content)
        {
            case Texture2D tex:
                ImgWriter.Write(tex, resourceWriter.BaseStream);
                tex.Dispose();
                break;

            case SpriteFont spr:
            {
                var tex = spr.Texture;
                ImgWriter.Write(tex, resourceWriter.BaseStream);
                var external = new ExternalSpriteFont(spr, Path.GetFileName(outputFilename));
                tex.Dispose();
                content = external;
                return;
            }
            case Effect eff:
                resourceWriter.BaseStream.Write(eff.Data);
                break;
            case TBin eff:
                resourceWriter.BaseStream.Write(eff.Data);
                break;
            case BmFont font:
                resourceWriter.Write(font.Xml);
                break;
        }

        content = Path.GetFileName(outputFilename);
    }
}
