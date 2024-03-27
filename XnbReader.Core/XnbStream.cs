using System.Text;
using K4os.Compression.LZ4;
using Serilog;
using XnbReader.Buffers;
using XnbReader.Decoder;
using XnbReader.FileFormat;

namespace XnbReader;

public sealed class XnbStream: Stream
{
    private Stream innerStream;

    public XnbStream(Stream stream)
    {
        innerStream = stream;
        File = Load();
        contentOffset = innerStream.CanSeek ? innerStream.Position : 0;
    }
    
    public XnbFile File { get; }

    public override int Read(byte[] buffer, int offset, int count) => innerStream.Read(buffer, offset, count);

    public override int Read(Span<byte> buffer) => innerStream.Read(buffer);
    public override int ReadByte() => innerStream.ReadByte();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    
    public override void Flush() => throw new NotSupportedException();

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => innerStream.Length - contentOffset;
    public override long Position { get; set; }
    
    private const int XnbCompressedPrologueSize = 14;
    private readonly long contentOffset;

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }
        
        innerStream.Dispose();
    }

    private XnbFile Load()
    {
        var reader = new BinaryReader(innerStream, Encoding.UTF8, true);
        
        if (innerStream is FileStream fileStream)
        {
            Log.Information("Reading file {filename:l}...", fileStream.Name);
        }
        
        // validate the XNB file header
        ValidateHeader(reader, out var header);

        // we validated the file successfully
        Log.Information("XNB file validated successfully!");

        // read the file size
        int fileSize = reader.ReadInt32();

        // verify the size
        if (fileSize != innerStream.Length)
        {
            throw new XnbException("XNB file has been truncated!");
        }

        // print out the file size
        Log.Debug("File size: {fileSize} bytes.", fileSize);


        // if the file is compressed then we need to decompress it
        if (header.Compressed)
        {
            // get the decompressed size
            int decompressedSize = reader.ReadInt32();
            Log.Debug("Uncompressed size: {decompressedSize} bytes.", decompressedSize);

            Stream decompressedStream = null!;

            switch (header.Flag)
            {
                // decompress LZX format
                case XnbFlag.Lzx:
                {
                    // decompress the buffer based on the file size
                    decompressedStream = new LzxDecoderStream(innerStream, decompressedSize, fileSize - XnbCompressedPrologueSize);
                    break;
                }
                // decompress LZ4 format
                case XnbFlag.Lz4:
                {
                    innerStream.Position = XnbCompressedPrologueSize;
                    // allocate buffer for LZ4 decode
                    using var compressedBytes = MemoryOwner<byte>.Allocate((int)(innerStream.Length - innerStream.Position));
                    _ = innerStream.Read(compressedBytes.Span);

                    // decode the trimmed buffer into decompressed buffer
                    byte[] bytes = new byte[decompressedSize];
                    LZ4Codec.Decode(compressedBytes.Span, bytes);
                    decompressedStream = new MemoryStream(bytes);
                    break;
                }
            }

            innerStream.Dispose();
            innerStream = decompressedStream;
            reader = new BinaryReader(innerStream, Encoding.UTF8, true);
        }

        // get the 7-bit value for readers
        int count = reader.Read7BitEncodedInt();
        // log how many readers there are
        Log.Debug("Readers: {count}", count);

        // a local copy of readers for the export
        var readers = new XnbTypeReader[count];

        // loop over the number of readers we have
        for (int i = 0; i < count; i++)
        {
            // read the type
            string type = reader.ReadString();
            // read the version
            int version = reader.ReadInt32();

            // add local reader
            readers[i] = new XnbTypeReader(type, version);
        }

        // get the 7-bit value for shared resources
        int shared = reader.Read7BitEncodedInt();

        // log the shared resources count
        Log.Debug("Shared Resources: {shared}", shared);

        // don't accept shared resources since SDV XNB files don't have any
        if (shared != 0)
        {
            throw new XnbException($"Unexpected ({shared}) shared resources.");
        }
        
        reader.Dispose();
        
        return new XnbFile(header, readers);
    }
    
    private void ValidateHeader(BinaryReader reader, out XnbHeader header)
    {
        // get the magic from the beginning of the file
        char[] magic = reader.ReadChars(3);

        // check to see if the magic is correct
        if (new string(magic) != "XNB")
        {
            throw new XnbException($"Invalid file magic found, expecting \"XNB\", found \"${magic}\"");
        }

        // debug print that valid XNB magic was found
        Log.Debug("Valid XNB magic found!");

        // load the target platform
        char target = char.ToLower(reader.ReadChar());

        // read the target platform
        switch (target)
        {
            case 'w':
                Log.Debug("Target platform: Microsoft Windows");
                break;
            case 'm':
                Log.Debug("Target platform: Windows Phone 7");
                break;
            case 'x':
                Log.Debug("Target platform: Xbox 360");
                break;
            case 'a':
                Log.Debug("Target platform: Android");
                break;
            case 'i':
                Log.Debug("Target platform: iOS");
                break;
            default:
                Log.Warning("Invalid target platform {target} found.", target);
                break;
        }

        // read the format version
        byte formatVersion = reader.ReadByte();

        // read the XNB format version
        switch (formatVersion)
        {
            case 0x3:
                Log.Debug("XNB Format Version: XNA Game Studio 3.0");
                break;
            case 0x4:
                Log.Debug("XNB Format Version: XNA Game Studio 3.1");
                break;
            case 0x5:
                Log.Debug("XNB Format Version: XNA Game Studio 4.0");
                break;
            default:
                Log.Warning("XNB Format Version 0x{0:X} unknown.", formatVersion);
                break;
        }

        // read the flag bits
        var flag = (XnbFlag)ReadByte();
        bool hidef = flag == XnbFlag.HiDef;

        flag &= ~XnbFlag.HiDef;
        
        // debug content information
        Log.Debug($"Content: {(hidef ? "HiDef" : "Reach")}");

        if (flag != XnbFlag.HiDef)
        {
            // log compressed state
            Log.Debug("Compressed: {compressed:l}", flag);
        }

        header = new XnbHeader(target, formatVersion, hidef, flag);
    }
}
