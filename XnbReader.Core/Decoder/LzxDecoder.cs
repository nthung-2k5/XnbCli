// Taken from MonoGame

#region HEADER
/* This file was derived from libmspack
 * (C) 2003-2004 Stuart Caie.
 * (C) 2011 Ali Scissons.
 *
 * The LZX method was created by Jonathan Forbes and Tomi Poutanen, adapted
 * by Microsoft Corporation.
 *
 * This source file is Dual licensed; meaning the end-user of this source file
 * may redistribute/modify it under the LGPL 2.1 or MS-PL licenses.
 */

#region LGPL License
/* GNU LESSER GENERAL PUBLIC LICENSE version 2.1
 * LzxDecoder is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 */
#endregion

#region MS-PL License
/*
 * MICROSOFT PUBLIC LICENSE
 * This source code is subject to the terms of the Microsoft Public License (Ms-PL).
 *
 * Redistribution and use in source and binary forms, with or without modification,
 * is permitted provided that redistributions of the source code retain the above
 * copyright notices and this file header.
 *
 * Additional copyright notices should be appended to the list above.
 *
 * For details, see <http://www.opensource.org/licenses/ms-pl.html>.
 */
#endregion

/*
 * This derived work is recognized by Stuart Caie and is authorized to adapt
 * any changes made to lzxd.c in his libmspack library and will still retain
 * this dual licensing scheme. Big thanks to Stuart Caie!
 *
 * DETAILS
 * This file is a pure C# port of the lzxd.c file from libmspack, with minor
 * changes towards the decompression of XNB files. The original decompression
 * software of LZX encoded data was written by Suart Caie in his
 * libmspack/cabextract projects, which can be located at
 * http://http://www.cabextract.org.uk/
 */
#endregion

using CommunityToolkit.HighPerformance.Buffers;

namespace XnbReader.Decoder;

internal class LzxDecoder : IDisposable
{
    private static uint[] PositionBase;
    private static byte[] ExtraBits;

    private LzxState mState;

    public LzxDecoder(int window)
    {
        uint windowSize = (uint)(1 << window);
        int posnSlots;

        // setup proper exception
        if (window is < 15 or > 21)
        {
            throw new UnsupportedWindowSizeRange();
        }

        // let's initialise our state
        mState = new LzxState();
        mState.Window = MemoryOwner<byte>.Allocate((int)windowSize);

        var windowSpan = mState.Window.Span;

        for (int i = 0; i < windowSize; i++)
        {
            windowSpan[i] = 0xDC;
        }

        mState.WindowSize = windowSize;
        mState.WindowPosn = 0;

        /* initialize static tables */
        if (ExtraBits == null)
        {
            ExtraBits = new byte[52];

            for (int i = 0, j = 0; i <= 50; i += 2)
            {
                ExtraBits[i] = ExtraBits[i + 1] = (byte)j;

                if (i != 0 && j < 17)
                {
                    j++;
                }
            }
        }

        if (PositionBase == null)
        {
            PositionBase = new uint[51];

            for (int i = 0, j = 0; i <= 50; i++)
            {
                PositionBase[i] = (uint)j;
                j += 1 << ExtraBits[i];
            }
        }

        /* calculate required position slots */
        if (window == 20)
        {
            posnSlots = 42;
        }
        else if (window == 21)
        {
            posnSlots = 50;
        }
        else
        {
            posnSlots = window << 1;
        }

        mState.R0 = mState.R1 = mState.R2 = 1;
        mState.MainElements = (ushort)(LzxConstants.NumChars + (posnSlots << 3));
        mState.HeaderRead = 0;
        mState.FramesRead = 0;
        mState.BlockRemaining = 0;
        mState.BlockType = LzxConstants.BlockType.Invalid;
        mState.IntelCurpos = 0;
        mState.IntelStarted = 0;

        // yo dawg i herd u liek arrays so we put arrays in ur arrays so u can array while u array
        mState.PretreeTable = new ushort[(1 << LzxConstants.PreTreeTableBits) + (LzxConstants.PreTreeMaxSymbols << 1)];
        mState.PretreeLen = new byte[LzxConstants.PreTreeMaxSymbols + LzxConstants.LenTableSafety];
        mState.MaintreeTable = new ushort[(1 << LzxConstants.MainTreeTableBits) + (LzxConstants.MainTreeMaxSymbols << 1)];
        mState.MaintreeLen = new byte[LzxConstants.MainTreeMaxSymbols + LzxConstants.LenTableSafety];
        mState.LengthTable = new ushort[(1 << LzxConstants.LengthTableBits) + (LzxConstants.LengthMaxSymbols << 1)];
        mState.LengthLen = new byte[LzxConstants.LengthMaxSymbols + LzxConstants.LenTableSafety];
        mState.AlignedTable = new ushort[(1 << LzxConstants.AlignedTableBits) + (LzxConstants.AlignedMaxSymbols << 1)];
        mState.AlignedLen = new byte[LzxConstants.AlignedMaxSymbols + LzxConstants.LenTableSafety];

        /* initialise tables to 0 (because deltas will be applied to them) */
        for (int i = 0; i < LzxConstants.MainTreeMaxSymbols; i++)
        {
            mState.MaintreeLen[i] = 0;
        }

        for (int i = 0; i < LzxConstants.LengthMaxSymbols; i++)
        {
            mState.LengthLen[i] = 0;
        }
    }

    public void Dispose()
    {
        mState.Dispose();
    }

    public int Decompress(Stream inData, int inLen, Stream outData, int outLen)
    {
        var bitbuf = new BitBuffer(inData);
        long startpos = inData.Position;
        long endpos = inData.Position + inLen;

        var window = mState.Window.Span;

        uint windowPosn = mState.WindowPosn;
        uint windowSize = mState.WindowSize;
        uint r0 = mState.R0;
        uint r1 = mState.R1;
        uint r2 = mState.R2;
        uint i, j;

        int togo = outLen;

        bitbuf.InitBitStream();

        /* read header if necessary */
        if (mState.HeaderRead == 0)
        {
            uint intel = bitbuf.ReadBits(1);

            if (intel != 0)
            {
                // read the filesize
                i = bitbuf.ReadBits(16);
                j = bitbuf.ReadBits(16);
                mState.IntelFilesize = (int)((i << 16) | j);
            }

            mState.HeaderRead = 1;
        }

        /* main decoding loop */
        while (togo > 0)
        {
            /* last block finished, new block expected */
            if (mState.BlockRemaining == 0)
            {
                // TODO may screw something up here
                if (mState.BlockType == LzxConstants.BlockType.Uncompressed)
                {
                    if ((mState.BlockLength & 1) == 1)
                    {
                        inData.ReadByte(); /* realign bitstream to word */
                    }

                    bitbuf.InitBitStream();
                }

                mState.BlockType = (LzxConstants.BlockType)bitbuf.ReadBits(3);
                
                i = bitbuf.ReadBits(16);
                j = bitbuf.ReadBits(8);
                mState.BlockRemaining = mState.BlockLength = (i << 8) | j;

                switch (mState.BlockType)
                {
                    case LzxConstants.BlockType.Aligned:
                        for (i = 0; i < 8; i++)
                        {
                            j = bitbuf.ReadBits(3);
                            mState.AlignedLen[i] = (byte)j;
                        }

                        MakeDecodeTable(LzxConstants.AlignedMaxSymbols, LzxConstants.AlignedTableBits, mState.AlignedLen, mState.AlignedTable);
                        /* rest of aligned header is same as verbatim */
                        goto case LzxConstants.BlockType.Verbatim;

                    case LzxConstants.BlockType.Verbatim:
                        ReadLengths(mState.MaintreeLen, 0, 256, bitbuf);
                        ReadLengths(mState.MaintreeLen, 256, mState.MainElements, bitbuf);
                        MakeDecodeTable(LzxConstants.MainTreeMaxSymbols, LzxConstants.MainTreeTableBits, mState.MaintreeLen, mState.MaintreeTable);

                        if (mState.MaintreeLen[0xE8] != 0)
                        {
                            mState.IntelStarted = 1;
                        }

                        ReadLengths(mState.LengthLen, 0, LzxConstants.NumSecondaryLengths, bitbuf);
                        MakeDecodeTable(LzxConstants.LengthMaxSymbols, LzxConstants.LengthTableBits, mState.LengthLen, mState.LengthTable);
                        break;

                    case LzxConstants.BlockType.Uncompressed:
                        mState.IntelStarted = 1; /* because we can't assume otherwise */
                        bitbuf.EnsureBits(16); /* get up to 16 pad bits into the buffer */

                        if (bitbuf.GetBitsLeft() > 16)
                        {
                            inData.Seek(-2, SeekOrigin.Current); /* and align the bitstream! */
                        }

                        byte lo = (byte)inData.ReadByte();
                        byte ml = (byte)inData.ReadByte();
                        byte mh = (byte)inData.ReadByte();
                        byte hi = (byte)inData.ReadByte();
                        r0 = (uint)(lo | (ml << 8) | (mh << 16) | (hi << 24));
                        lo = (byte)inData.ReadByte();
                        ml = (byte)inData.ReadByte();
                        mh = (byte)inData.ReadByte();
                        hi = (byte)inData.ReadByte();
                        r1 = (uint)(lo | (ml << 8) | (mh << 16) | (hi << 24));
                        lo = (byte)inData.ReadByte();
                        ml = (byte)inData.ReadByte();
                        mh = (byte)inData.ReadByte();
                        hi = (byte)inData.ReadByte();
                        r2 = (uint)(lo | (ml << 8) | (mh << 16) | (hi << 24));
                        break;

                    default:
                        return -1; // TODO throw proper exception
                }
            }

            /* buffer exhaustion check */
            if (inData.Position > startpos + inLen)
            {
                /* it's possible to have a file where the next run is less than
                 * 16 bits in size. In this case, the READ_HUFFSYM() macro used
                 * in building the tables will exhaust the buffer, so we should
                 * allow for this, but not allow those accidentally read bits to
                 * be used (so we check that there are at least 16 bits
                 * remaining - in this boundary case they aren't really part of
                 * the compressed data)
                 */
                //Debug.WriteLine("WTF");

                if (inData.Position > startpos + inLen + 2 || bitbuf.GetBitsLeft() < 16)
                {
                    return -1; //TODO throw proper exception
                }
            }

            int thisRun;

            while ((thisRun = (int)mState.BlockRemaining) > 0 && togo > 0)
            {
                if (thisRun > togo)
                {
                    thisRun = togo;
                }

                togo -= thisRun;
                mState.BlockRemaining -= (uint)thisRun;

                /* apply 2^x-1 mask */
                windowPosn &= windowSize - 1;

                /* runs can't straddle the window wraparound */
                if (windowPosn + thisRun > windowSize)
                {
                    return -1; //TODO throw proper exception
                }
                
                int mainElement, matchLength, matchOffset, lengthFooter, extra, verbatimBits;
                int rundest, runsrc, copyLength;

                switch (mState.BlockType)
                {
                    case LzxConstants.BlockType.Verbatim:
                        while (thisRun > 0)
                        {
                            mainElement = (int)ReadHuffSym(mState.MaintreeTable, mState.MaintreeLen, LzxConstants.MainTreeMaxSymbols, LzxConstants.MainTreeTableBits, bitbuf);

                            if (mainElement < LzxConstants.NumChars)
                            {
                                /* literal: 0 to NUM_CHARS-1 */
                                window[(int)windowPosn++] = (byte)mainElement;
                                thisRun--;
                            }
                            else
                            {
                                /* match: NUM_CHARS + ((slot<<3) | length_header (3 bits)) */
                                mainElement -= LzxConstants.NumChars;

                                matchLength = mainElement & LzxConstants.NumPrimaryLengths;

                                if (matchLength == LzxConstants.NumPrimaryLengths)
                                {
                                    lengthFooter = (int)ReadHuffSym(mState.LengthTable, mState.LengthLen, LzxConstants.LengthMaxSymbols, LzxConstants.LengthTableBits, bitbuf);
                                    matchLength += lengthFooter;
                                }

                                matchLength += LzxConstants.MinMatch;

                                matchOffset = mainElement >> 3;

                                if (matchOffset > 2)
                                {
                                    /* not repeated offset */
                                    if (matchOffset != 3)
                                    {
                                        extra = ExtraBits[matchOffset];
                                        verbatimBits = (int)bitbuf.ReadBits((byte)extra);
                                        matchOffset = (int)PositionBase[matchOffset] - 2 + verbatimBits;
                                    }
                                    else
                                    {
                                        matchOffset = 1;
                                    }

                                    /* update repeated offset LRU queue */
                                    r2 = r1;
                                    r1 = r0;
                                    r0 = (uint)matchOffset;
                                }
                                else if (matchOffset == 0)
                                {
                                    matchOffset = (int)r0;
                                }
                                else if (matchOffset == 1)
                                {
                                    matchOffset = (int)r1;
                                    r1 = r0;
                                    r0 = (uint)matchOffset;
                                }
                                else /* match_offset == 2 */
                                {
                                    matchOffset = (int)r2;
                                    r2 = r0;
                                    r0 = (uint)matchOffset;
                                }

                                rundest = (int)windowPosn;
                                thisRun -= matchLength;

                                /* copy any wrapped around source data */
                                if (windowPosn >= matchOffset)
                                {
                                    /* no wrap */
                                    runsrc = rundest - matchOffset;
                                }
                                else
                                {
                                    runsrc = rundest + ((int)windowSize - matchOffset);
                                    copyLength = matchOffset - (int)windowPosn;

                                    if (copyLength < matchLength)
                                    {
                                        matchLength -= copyLength;
                                        windowPosn += (uint)copyLength;

                                        while (copyLength-- > 0)
                                        {
                                            window[rundest++] = window[runsrc++];
                                        }

                                        runsrc = 0;
                                    }
                                }

                                windowPosn += (uint)matchLength;

                                /* copy match data - no worries about destination wraps */
                                while (matchLength-- > 0)
                                {
                                    window[rundest++] = window[runsrc++];
                                }
                            }
                        }

                        break;

                    case LzxConstants.BlockType.Aligned:
                        while (thisRun > 0)
                        {
                            mainElement = (int)ReadHuffSym(mState.MaintreeTable, mState.MaintreeLen, LzxConstants.MainTreeMaxSymbols, LzxConstants.MainTreeTableBits, bitbuf);

                            if (mainElement < LzxConstants.NumChars)
                            {
                                /* literal 0 to NUM_CHARS-1 */
                                window[(int)windowPosn++] = (byte)mainElement;
                                thisRun--;
                            }
                            else
                            {
                                /* match: NUM_CHARS + ((slot<<3) | length_header (3 bits)) */
                                mainElement -= LzxConstants.NumChars;

                                matchLength = mainElement & LzxConstants.NumPrimaryLengths;

                                if (matchLength == LzxConstants.NumPrimaryLengths)
                                {
                                    lengthFooter = (int)ReadHuffSym(mState.LengthTable, mState.LengthLen, LzxConstants.LengthMaxSymbols, LzxConstants.LengthTableBits, bitbuf);
                                    matchLength += lengthFooter;
                                }

                                matchLength += LzxConstants.MinMatch;

                                matchOffset = mainElement >> 3;

                                switch (matchOffset)
                                {
                                    case > 2:
                                    {
                                        /* not repeated offset */
                                        extra = ExtraBits[matchOffset];
                                        matchOffset = (int)PositionBase[matchOffset] - 2;

                                        int alignedBits;

                                        if (extra > 3)
                                        {
                                            /* verbatim and aligned bits */
                                            extra -= 3;
                                            verbatimBits = (int)bitbuf.ReadBits((byte)extra);
                                            matchOffset += verbatimBits << 3;
                                            alignedBits = (int)ReadHuffSym(mState.AlignedTable, mState.AlignedLen, LzxConstants.AlignedMaxSymbols, LzxConstants.AlignedTableBits, bitbuf);
                                            matchOffset += alignedBits;
                                        }
                                        else if (extra == 3)
                                        {
                                            /* aligned bits only */
                                            alignedBits = (int)ReadHuffSym(mState.AlignedTable, mState.AlignedLen, LzxConstants.AlignedMaxSymbols, LzxConstants.AlignedTableBits, bitbuf);
                                            matchOffset += alignedBits;
                                        }
                                        else if (extra > 0) /* extra==1, extra==2 */
                                        {
                                            /* verbatim bits only */
                                            verbatimBits = (int)bitbuf.ReadBits((byte)extra);
                                            matchOffset += verbatimBits;
                                        }
                                        else /* extra == 0 */
                                        {
                                            /* ??? */
                                            matchOffset = 1;
                                        }

                                        /* update repeated offset LRU queue */
                                        r2 = r1;
                                        r1 = r0;
                                        r0 = (uint)matchOffset;

                                        break;
                                    }
                                    case 0:
                                        matchOffset = (int)r0;

                                        break;
                                    case 1:
                                        matchOffset = (int)r1;
                                        r1 = r0;
                                        r0 = (uint)matchOffset;

                                        break;
                                    /* match_offset == 2 */
                                    default:
                                        matchOffset = (int)r2;
                                        r2 = r0;
                                        r0 = (uint)matchOffset;

                                        break;
                                }

                                rundest = (int)windowPosn;
                                thisRun -= matchLength;

                                /* copy any wrapped around source data */
                                if (windowPosn >= matchOffset)
                                {
                                    /* no wrap */
                                    runsrc = rundest - matchOffset;
                                }
                                else
                                {
                                    runsrc = rundest + ((int)windowSize - matchOffset);
                                    copyLength = matchOffset - (int)windowPosn;

                                    if (copyLength < matchLength)
                                    {
                                        matchLength -= copyLength;
                                        windowPosn += (uint)copyLength;

                                        while (copyLength-- > 0)
                                        {
                                            window[rundest++] = window[runsrc++];
                                        }

                                        runsrc = 0;
                                    }
                                }

                                windowPosn += (uint)matchLength;

                                /* copy match data - no worries about destination wraps */
                                while (matchLength-- > 0)
                                {
                                    window[rundest++] = window[runsrc++];
                                }
                            }
                        }

                        break;

                    case LzxConstants.BlockType.Uncompressed:
                        if (inData.Position + thisRun > endpos)
                        {
                            return -1; //TODO throw proper exception
                        }

                        byte[] tempBuffer = new byte[thisRun];
                        _ = inData.Read(tempBuffer);
                        tempBuffer.CopyTo(window[..(int)windowPosn]);
                        windowPosn += (uint)thisRun;
                        break;

                    default:
                        return -1; //TODO throw proper exception
                }
            }
        }

        if (togo != 0)
        {
            return -1; //TODO throw proper exception
        }

        int startWindowPos = (int)windowPosn;

        if (startWindowPos == 0)
        {
            startWindowPos = (int)windowSize;
        }

        startWindowPos -= outLen;
        outData.Write(window.Slice(startWindowPos, outLen));

        mState.WindowPosn = windowPosn;
        mState.R0 = r0;
        mState.R1 = r1;
        mState.R2 = r2;

        // TODO finish intel E8 decoding
        /* intel E8 decoding */
        if (mState.FramesRead++ < 32768 && mState.IntelFilesize != 0)
        {
            if (outLen <= 6 || mState.IntelStarted == 0)
            {
                mState.IntelCurpos += outLen;
            }
            else
            {
                int dataEnd = outLen - 10;
                uint currentPos = (uint)mState.IntelCurpos;

                mState.IntelCurpos = (int)currentPos + outLen;

                while (outData.Position < dataEnd)
                {
                    if (outData.ReadByte() != 0xE8)
                    {
                        currentPos++;
                    }
                }
            }

            return -1;
        }

        return 0;
    }

    // READ_LENGTHS(table, first, last)
    // if(lzx_read_lens(LENTABLE(table), first, last, bitsleft))
    //   return ERROR (ILLEGAL_DATA)
    // 

    // TODO make returns throw exceptions
    private static void MakeDecodeTable(uint nsyms, uint nbits, byte[] length, ushort[] table)
    {
        ushort sym;
        uint leaf;
        byte bitNum = 1;
        uint fill;
        uint pos = 0; /* the current position in the decode table */
        uint tableMask = (uint)(1 << (int)nbits);
        uint bitMask = tableMask >> 1; /* don't do 0 length codes */
        uint nextSymbol = bitMask; /* base of allocation for long codes */

        /* fill entries for codes short enough for a direct mapping */
        while (bitNum <= nbits)
        {
            for (sym = 0; sym < nsyms; sym++)
            {
                if (length[sym] == bitNum)
                {
                    leaf = pos;

                    if ((pos += bitMask) > tableMask)
                    {
                        return;
                    }

                    /* fill all possible lookups of this symbol with the symbol itself */
                    fill = bitMask;

                    while (fill-- > 0)
                    {
                        table[leaf++] = sym;
                    }
                }
            }

            bitMask >>= 1;
            bitNum++;
        }

        /* if there are any codes longer than nbits */
        if (pos != tableMask)
        {
            /* clear the remainder of the table */
            for (sym = (ushort)pos; sym < tableMask; sym++)
            {
                table[sym] = 0;
            }

            /* give ourselves room for codes to grow by up to 16 more bits */
            pos <<= 16;
            tableMask <<= 16;
            bitMask = 1 << 15;

            while (bitNum <= 16)
            {
                for (sym = 0; sym < nsyms; sym++)
                {
                    if (length[sym] == bitNum)
                    {
                        leaf = pos >> 16;

                        for (fill = 0; fill < bitNum - nbits; fill++)
                        {
                            /* if this path hasn't been taken yet, 'allocate' two entries */
                            if (table[leaf] == 0)
                            {
                                table[nextSymbol << 1] = 0;
                                table[(nextSymbol << 1) + 1] = 0;
                                table[leaf] = (ushort)nextSymbol++;
                            }

                            /* follow the path and select either left or right for next bit */
                            leaf = (uint)(table[leaf] << 1);

                            if (((pos >> (int)(15 - fill)) & 1) == 1)
                            {
                                leaf++;
                            }
                        }

                        table[leaf] = sym;

                        if ((pos += bitMask) > tableMask)
                        {
                            return;
                        }
                    }
                }

                bitMask >>= 1;
                bitNum++;
            }
        }

        /* full talbe? */
        if (pos == tableMask)
        {
            return;
        }

        /* either erroneous table, or all elements are 0 - let's find out. */
        for (sym = 0; sym < nsyms; sym++)
        {
            if (length[sym] != 0)
            {
                return;
            }
        }
    }

    // TODO throw exceptions instead of returns
    private void ReadLengths(byte[] lens, uint first, uint last, BitBuffer buffer)
    {
        uint x, y;

        // hufftbl pointer here?

        for (x = 0; x < 20; x++)
        {
            y = buffer.ReadBits(4);
            mState.PretreeLen[x] = (byte)y;
        }

        MakeDecodeTable(LzxConstants.PreTreeMaxSymbols, LzxConstants.PreTreeTableBits, mState.PretreeLen, mState.PretreeTable);

        for (x = first; x < last;)
        {
            int z = (int)ReadHuffSym(mState.PretreeTable, mState.PretreeLen, LzxConstants.PreTreeMaxSymbols, LzxConstants.PreTreeTableBits, buffer);

            if (z == 17)
            {
                y = buffer.ReadBits(4);
                y += 4;

                while (y-- != 0)
                {
                    lens[x++] = 0;
                }
            }
            else if (z == 18)
            {
                y = buffer.ReadBits(5);
                y += 20;

                while (y-- != 0)
                {
                    lens[x++] = 0;
                }
            }
            else if (z == 19)
            {
                y = buffer.ReadBits(1);
                y += 4;
                z = (int)ReadHuffSym(mState.PretreeTable, mState.PretreeLen, LzxConstants.PreTreeMaxSymbols, LzxConstants.PreTreeTableBits, buffer);
                z = lens[x] - z;

                if (z < 0)
                {
                    z += 17;
                }

                while (y-- != 0)
                {
                    lens[x++] = (byte)z;
                }
            }
            else
            {
                z = lens[x] - z;

                if (z < 0)
                {
                    z += 17;
                }

                lens[x++] = (byte)z;
            }
        }
    }

    private static uint ReadHuffSym(ushort[] table, byte[] lengths, uint nsyms, uint nbits, BitBuffer bitBuffer)
    {
        uint i, j;
        bitBuffer.EnsureBits(16);

        if ((i = table[bitBuffer.PeekBits((byte)nbits)]) >= nsyms)
        {
            j = (uint)(1 << (int)(sizeof(uint) * 8 - nbits));

            do
            {
                j >>= 1;
                i <<= 1;
                i |= (bitBuffer.GetBuffer() & j) != 0 ? (uint)1 : 0;

                if (j == 0)
                {
                    return 0; // TODO throw proper exception
                }
            } while ((i = table[i]) >= nsyms);
        }

        j = lengths[i];
        bitBuffer.RemoveBits((byte)j);

        return i;
    }

#region Our BitBuffer Class
    private class BitBuffer
    {
        private readonly Stream byteStream;
        private byte bitsleft;
        private uint buffer;

        public BitBuffer(Stream stream)
        {
            byteStream = stream;
            InitBitStream();
        }

        public void InitBitStream()
        {
            buffer = 0;
            bitsleft = 0;
        }

        public void EnsureBits(byte bits)
        {
            while (bitsleft < bits)
            {
                int lo = (byte)byteStream.ReadByte();
                int hi = (byte)byteStream.ReadByte();
                //int amount2shift = sizeof(uint)*8 - 16 - bitsleft;
                buffer |= (uint)(((hi << 8) | lo) << (sizeof(uint) * 8 - 16 - bitsleft));
                bitsleft += 16;
            }
        }

        public uint PeekBits(byte bits) => buffer >> (sizeof(uint) * 8 - bits);

        public void RemoveBits(byte bits)
        {
            buffer <<= bits;
            bitsleft -= bits;
        }

        public uint ReadBits(byte bits)
        {
            uint ret = 0;

            if (bits > 0)
            {
                EnsureBits(bits);
                ret = PeekBits(bits);
                RemoveBits(bits);
            }

            return ret;
        }

        public uint GetBuffer() => buffer;

        public byte GetBitsLeft() => bitsleft;
    }
#endregion

    private struct LzxState : IDisposable
    {
        public uint R0, R1, R2; /* for the LRU offset system				*/
        public ushort MainElements; /* number of main tree elements				*/
        public int HeaderRead; /* have we started decoding at all yet? 	*/
        public LzxConstants.BlockType BlockType; /* type of this block						*/
        public uint BlockLength; /* uncompressed length of this block 		*/
        public uint BlockRemaining; /* uncompressed bytes still left to decode	*/
        public uint FramesRead; /* the number of CFDATA blocks processed	*/
        public int IntelFilesize; /* magic header value used for transform	*/
        public int IntelCurpos; /* current offset in transform space		*/
        public int IntelStarted; /* have we seen any translateable data yet?	*/

        public ushort[] PretreeTable;
        public byte[] PretreeLen;
        public ushort[] MaintreeTable;
        public byte[] MaintreeLen;
        public ushort[] LengthTable;
        public byte[] LengthLen;
        public ushort[] AlignedTable;
        public byte[] AlignedLen;

        // NEEDED MEMBERS
        // CAB actualsize
        // CAB window
        // CAB window_size
        // CAB window_posn
        public MemoryOwner<byte> Window;
        public uint WindowSize;
        public uint WindowPosn;

        public void Dispose()
        {
            Window?.Dispose();
        }
    }
}

/* CONSTANTS */
internal struct LzxConstants
{
    public const ushort MinMatch = 2;
    public const ushort MaxMatch = 257;
    public const ushort NumChars = 256;

    public enum BlockType
    {
        Invalid = 0,
        Verbatim = 1,
        Aligned = 2,
        Uncompressed = 3
    }

    private const ushort PreTreeNumElements = 20;
    private const ushort AlignedNumElements = 8;
    public const ushort NumPrimaryLengths = 7;
    public const ushort NumSecondaryLengths = 249;

    public const ushort PreTreeMaxSymbols = PreTreeNumElements;
    public const ushort PreTreeTableBits = 6;
    public const ushort MainTreeMaxSymbols = NumChars + 50 * 8;
    public const ushort MainTreeTableBits = 12;
    public const ushort LengthMaxSymbols = NumSecondaryLengths + 1;
    public const ushort LengthTableBits = 12;
    public const ushort AlignedMaxSymbols = AlignedNumElements;
    public const ushort AlignedTableBits = 7;

    public const ushort LenTableSafety = 64;
}

/* EXCEPTIONS */
public class UnsupportedWindowSizeRange : Exception;
