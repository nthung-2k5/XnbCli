// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.IO;

namespace Xnb.Decoder;

internal class LzxDecoderStream : Stream
{
    private LzxDecoder dec;
    private RecyclableMemoryStream decompressedStream;
    
    private static readonly RecyclableMemoryStreamManager Manager = new();

    public LzxDecoderStream(Stream input, int decompressedSize, int compressedSize)
    {
        dec = new LzxDecoder(16);

        // TODO: Rewrite using block decompression like Lz4DecoderStream
        Decompress(input, decompressedSize, compressedSize);
    }

    // Decompress into MemoryStream
    private void Decompress(Stream stream, int decompressedSize, int compressedSize)
    {
        //thanks to ShinAli (https://bitbucket.org/alisci01/xnbdecompressor)
        // default window size for XNB encoded files is 64Kb (need 16 bits to represent it)
        decompressedStream = Manager.GetStream();
        long startPos = stream.Position;
        long pos = startPos;
            
        while (pos - startPos < compressedSize)
        {
            // the compressed stream is seperated into blocks that will decompress
            // into 32Kb or some other size if specified.
            // normal, 32Kb output blocks will have a short indicating the size
            // of the block before the block starts
            // blocks that have a defined output will be preceded by a byte of value
            // 0xFF (255), then a short indicating the output size and another
            // for the block size
            // all shorts for these cases are encoded in big endian order
            int hi = stream.ReadByte();
            int lo = stream.ReadByte();
            int blockSize = (hi << 8) | lo;
            int frameSize = 0x8000; // frame size is 32Kb by default
            // does this block define a frame size?
            if (hi == 0xFF)
            {
                hi = lo;
                lo = (byte)stream.ReadByte();
                frameSize = (hi << 8) | lo;
                hi = (byte)stream.ReadByte();
                lo = (byte)stream.ReadByte();
                blockSize = (hi << 8) | lo;
                pos += 5;
            }
            else
                pos += 2;

            // either says there is nothing to decode
            if (blockSize == 0 || frameSize == 0)
                break;

            dec.Decompress(stream, blockSize, decompressedStream, frameSize);
            pos += blockSize;

            // reset the position of the input just in case the bit buffer
            // read in some unused bytes
            stream.Seek(pos, SeekOrigin.Begin);
        }

        if (decompressedStream.Position != decompressedSize)
        {
            throw new XnbException("Decompression failed.");
        }

        decompressedStream.Seek(0, SeekOrigin.Begin);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if(disposing)
        {                
            decompressedStream.Dispose();
            dec.Dispose();
        }            
        dec = null;
        decompressedStream = null;
    }

#region Stream internals

    public override int Read(byte[] buffer, int offset, int count)
    {
        return decompressedStream.Read(buffer, offset, count);
    }
    
    public override int Read(Span<byte> buffer)
    {
        return decompressedStream.Read(buffer);
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override void Flush()
    {
    }

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }


    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

#endregion
}