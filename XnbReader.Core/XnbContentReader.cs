using System.Text;
using K4os.Compression.LZ4;
using Serilog;
using XnbReader.Decoder;
using XnbReader.FileFormat;

namespace XnbReader;

public abstract class XnbContentReader(XnbStream input) : BinaryReader(input)
{
    public abstract object Read(string readerType);
    
    public object LoadObject(bool loadIntoXnbFile = true)
    {
        var stream = (XnbStream)BaseStream;
        content ??= Read(TypeResolver.SimplifyType(stream.File.Readers[0].Type));
        
        if (loadIntoXnbFile && stream.File.Content is null)
        {
            stream.File.Content = content;
        }
        
        // we loaded the XNB file successfully
        Log.Information("Successfully read XNB file!");
        
        return content;
    }

    private object? content;
}
