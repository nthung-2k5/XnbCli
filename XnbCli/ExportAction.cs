using System.Text.Encodings.Web;
using System.Text.Json;
using Serilog;
using StbImageWriteSharp;
using XnbCli.TextureHelper;
using XnbReader.FileFormat;
using XnbReader.MonoGameShims;

namespace XnbCli;

public static class ExportAction
{
    private static readonly ImageWriter ImgWriter = new();

    public static void ExportFile(string filename, XnbFile xnb)
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

        ExportExternalResource(filename, ref xnb.Content);

        using var json = File.Create(filename);
        using var utf8Json = new Utf8JsonWriter(json, new JsonWriterOptions { Indented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

        JsonSerializer.Serialize(utf8Json, xnb, JsonContextForXnbReader.Default.XnbFile);

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
        var stream = resourceWriter.BaseStream;

        switch (content)
        {
            case Texture2D tex:
                ExportTextureAndDispose(tex, stream);
                break;

            case SpriteFont spr:
                ExportTextureAndDispose(spr.Texture, stream);
                content = new ExternalSpriteFont(spr, Path.GetFileName(outputFilename));
                return;
            case Effect eff:
                stream.Write(eff.Data);
                break;
            case TBin tbin:
                stream.Write(tbin.Data);
                break;
            case BmFont font:
                resourceWriter.Write(font.Xml);
                break;
        }

        content = Path.GetFileName(outputFilename);
    }

    private static void ExportTextureAndDispose(Texture2D tex, Stream stream)
    {
        ImgWriter.Write(tex, stream);
        tex.Dispose();
    }
}
