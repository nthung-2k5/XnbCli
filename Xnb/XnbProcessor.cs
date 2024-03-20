using K4os.Compression.LZ4;
using Serilog;
using Xnb.Decoder;

namespace Xnb;

public static class XnbProcessor
{
    // ReSharper disable InconsistentNaming
    private const int HiDefMask = 0x1;
    private const int CompressedLZ4Mask = 0x40;
    private const int CompressedLZXMask = 0x80;
    // ReSharper restore InconsistentNaming
    private const int XnbCompressedPrologueSize = 14;

    public static XnbFile Load(string filename)
    {
        Log.Information("Reading file {filename:l}...", filename);

        // create a new instance of reader
        using var file = File.OpenRead(filename);
        var buffer = new BinaryReader(file);

        // validate the XNB file header
        ValidateHeader(buffer, out var header);

        // we validated the file successfully
        Log.Information("XNB file validated successfully!");

        // read the file size
        int fileSize = buffer.ReadInt32();

        // verify the size
        if (fileSize != file.Length)
        {
            throw new XnbException("XNB file has been truncated!");
        }

        // print out the file size
        Log.Debug("File size: {fileSize} bytes.", fileSize);

        var xnb = new XnbFile { Header = header };
        Stream decompressedStream = file;

        // if the file is compressed then we need to decompress it
        if (header.Compressed)
        {
            // get the decompressed size
            int decompressedSize = buffer.ReadInt32();

            Stream decompressed;
            Log.Debug("Uncompressed size: {decompressedSize} bytes.", decompressedSize);

            switch (header.CompressionType)
            {
                // decompress LZX format
                case CompressedLZXMask:
                {
                    // decompress the buffer based on the file size
                    decompressed = new LzxDecoderStream(buffer.BaseStream, decompressedSize, fileSize - XnbCompressedPrologueSize);

                    break;
                }
                // decompress LZ4 format
                default: // case COMPRESSED_LZ4_MASK:
                {
                    byte[] bytes = new byte[decompressedSize];
                    // allocate buffer for LZ4 decode
                    file.Position = XnbCompressedPrologueSize;
                    using var trimmed = new MemoryStream();
                    file.CopyTo(trimmed);
                    var trimmedSpan = trimmed.GetBuffer().AsSpan()[..(int)trimmed.Length];

                    // decode the trimmed buffer into decompressed buffer
                    LZ4Codec.Decode(trimmedSpan, bytes);
                    decompressed = new MemoryStream(bytes);
                    break;
                }
            }

            buffer.Dispose();
            decompressedStream = decompressed;
        }

        buffer = new BinaryReader(decompressedStream);

        // Log.Debug("Reading from byte position: {bytePosition}", buffer.BaseStream.Position);

        // NOTE: assuming the buffer is now decompressed

        // get the 7-bit value for readers
        int count = buffer.Read7BitEncodedInt();
        // log how many readers there are
        Log.Debug("Readers: {count}", count);

        // a local copy of readers for the export
        xnb.Readers = new XnbReader[count];

        // loop over the number of readers we have
        for (int i = 0; i < count; i++)
        {
            // read the type
            string type = buffer.ReadString();
            // read the version
            int version = buffer.ReadInt32();

            // add local reader
            xnb.Readers[i] = new XnbReader(type, version);
        }

        // get the 7-bit value for shared resources
        int shared = buffer.Read7BitEncodedInt();

        // log the shared resources count
        Log.Debug("Shared Resources: {shared}", shared);

        // don't accept shared resources since SDV XNB files don't have any
        if (shared != 0)
        {
            throw new XnbException($"Unexpected ({shared}) shared resources.");
        }

        // create content reader from the first reader and read the content in
        //var reader = new TestReader(buffer.BaseStream);
        //xnb.Content = reader.Read(TypeResolver.SimplifyType(xnb.Readers[0].Type));

        // we loaded the XNB file successfully
        Log.Information("Successfuly read XNB file!");

        buffer.Dispose();
        return xnb;
    }

    private static void ValidateHeader(BinaryReader buffer, out XnbHeader header)
    {
        // ensure buffer isn't null
        if (buffer == null)
        {
            throw new XnbException("Buffer is null");
        }

        // get the magic from the beginning of the file
        char[] magic = buffer.ReadChars(3);

        // check to see if the magic is correct
        if (new string(magic) != "XNB")
        {
            throw new XnbException($"Invalid file magic found, expecting \"XNB\", found \"${magic}\"");
        }

        // debug print that valid XNB magic was found
        Log.Debug("Valid XNB magic found!");

        // load the target platform
        char target = char.ToLower(buffer.ReadChar());

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
        byte formatVersion = buffer.ReadByte();

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
        byte flags = buffer.ReadByte();
        // get the HiDef flag
        bool hidef = (flags & HiDefMask) != 0;
        // get the compressed flag
        bool compressed = ((flags & CompressedLZXMask) | (flags & CompressedLZ4Mask)) != 0;
        // set the compression type
        // NOTE: probably a better way to do both lines but sticking with this for now
        int compressionType = (flags & CompressedLZXMask) != 0 ? CompressedLZXMask : (flags & CompressedLZ4Mask) != 0 ? CompressedLZ4Mask : 0;
        // debug content information
        Log.Debug($"Content: {(hidef ? "HiDef" : "Reach")}");
        // log compressed state
        Log.Debug("Compressed: {compressed:l}, {compressionType:l}", compressed, compressionType == CompressedLZXMask ? "LZX" : "LZ4");

        header = new XnbHeader(target, formatVersion, hidef, compressed, compressionType);
    }
}
