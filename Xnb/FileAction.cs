using System.Text.Encodings.Web;
using System.Text.Json;
using Serilog;
using StbImageWriteSharp;
using Xnb.Reader;
using Xnb.Reader.ContentReader;
using Xnb.Types;

namespace Xnb;

public static class FileAction
{
    private static readonly ImageWriter ImgWriter = new();
    public static void ExportFile(string filename, XnbFile xnb)
    {
        string dirPath = Path.GetDirectoryName(filename)!;

        if (!string.IsNullOrEmpty(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        object temp = xnb.Content;
        ExportResource(filename, ref temp);
        xnb.Content = temp;
        
        using var json = File.Create(filename);
        using var utf8Json = new Utf8JsonWriter(json, new JsonWriterOptions { Indented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        
        JsonSerializer.Serialize(utf8Json, xnb, SourceGenerationContext.Default.XnbFile);

        if (xnb.Content is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private static void ExportResource(string filename, ref object content)
    {
        if (!content.GetType().IsSubclassOfRawGeneric(typeof(ExplicitType<>)))
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
            _ => "bin"
        };
            
        string outputFilename = Path.ChangeExtension(filename, extension);
        var resourceFile = new FileInfo(outputFilename);
        using var resourceWriter = resourceFile.CreateText();
            
        switch (content)
        {
            case Texture2D tex:
                ImgWriter.WritePng(tex.Data, tex.Width, tex.Height, ColorComponents.RedGreenBlueAlpha, resourceWriter.BaseStream);
                tex.Dispose();

                break;
            case SpriteFont spr:
            {
                var tex = spr.Data;
                ImgWriter.WritePng(tex.Data, tex.Width, tex.Height, ColorComponents.RedGreenBlueAlpha, resourceWriter.BaseStream);
                var external = new ExternalSpriteFont(spr, Path.GetFileName(outputFilename));
                tex.Dispose();
                content = external;
            
                return;
            }
            default:
                object buffer = content.GetType().GetProperty("Data")!.GetValue(content);
                switch (buffer)
                {
                    case byte[] bytes:
                        resourceWriter.BaseStream.Write(bytes);
                        break;
                    case string xml:
                        resourceWriter.Write(xml);
                        break;
                }

                break;
        }
        
        content = Path.GetFileName(outputFilename);
    }
}
