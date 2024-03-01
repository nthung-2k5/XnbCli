using CommunityToolkit.HighPerformance.Buffers;
using Serilog;
using Xnb.Decoder;
using Xnb.Types;

namespace Xnb.Reader.ContentReader;

public class Texture2DReader: BaseReader<Texture2D>
{
    public override Texture2D Read(BinaryReader buffer)
    {
	    var surfaceFormat = (SurfaceFormat)ReaderResolver.Read<int>(buffer);
        int width = ReaderResolver.Read<int>(buffer);
        int height = ReaderResolver.Read<int>(buffer);
        int levelCount = ReaderResolver.Read<int>(buffer);
        
        if (levelCount > 1)
        {
	        Log.Warning("Found mipcount of {0}, only the first will be used.", levelCount);
        }

        var dataOwner = ReaderResolver.Read<MemoryOwner<byte>>(buffer);
        var data = dataOwner.Span;

	    //Convert the image data if required
	    switch (surfaceFormat)
	    {
		    case SurfaceFormat.Dxt1:
		    case SurfaceFormat.Dxt3:
		    case SurfaceFormat.Dxt5:
			    var decompressed = DxtUtil.Decompress(data, width, height, surfaceFormat);
				dataOwner.Dispose();
			    
			    dataOwner = decompressed;
			    data = dataOwner.Span;
			    break;
		    case SurfaceFormat.Color:
			    break;
		    case SurfaceFormat.Bgra5551:
			    throw new XnbException("Texture2D format type ECT1 not implemented!");
		    default:
			    throw new XnbException($"Non-implemented Texture2D format type ({surfaceFormat}) found.");
	    }

	    // add the alpha channel into the image
	    for (int i = 0; i < data.Length; i += 4)
	    {
		    float inverseAlpha = 255f / data[i + 3];
		    data[i] = (byte)Math.Min(MathF.Ceiling(data[i] * inverseAlpha), 255);
		    data[i + 1] = (byte)Math.Min(MathF.Ceiling(data[i + 1] * inverseAlpha), 255);
		    data[i + 2] = (byte)Math.Min(MathF.Ceiling(data[i + 2] * inverseAlpha), 255);
	    }
		return new Texture2D(surfaceFormat, width, height) { DataOwner = dataOwner };
    }

    public override void Write(BinaryWriter buffer, Texture2D content)
    {
        throw new NotImplementedException();
    }
}
