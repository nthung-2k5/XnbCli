// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Text;

namespace XnbCli.CecilPorter;

internal sealed class SourceWriter
{
    private readonly StringBuilder sb = new();

    public int Indentation { get; private set; }

    public SourceWriter Indent(bool withBrace = true)
    {
        if (withBrace)
        {
            WriteLine('{');
        }

        ++Indentation;
        return this;
    }
    
    public SourceWriter Dedent(bool withBrace = true)
    {
        --Indentation;
        
        if (withBrace)
        {
            WriteLine('}');
        }
        
        return this;
    }

    public SourceWriter WriteLine(char value)
    {
        sb.AddIndentation(Indentation)
          .Append(value)
          .AppendLine();
        return this;
    }

    public SourceWriter WriteLine(string text)
    {
        if (Indentation == 0)
        {
            sb.AppendLine(text);
        }
        else
        {
            bool isFinalLine;
            var remainingText = text.AsSpan();
            do
            {
                var nextLine = GetNextLine(ref remainingText, out isFinalLine);

                sb.AddIndentation(Indentation)
                  .AppendSpan(nextLine)
                  .AppendLine();
            }
            while (!isFinalLine);
        }

        return this;
    }

    public SourceWriter WriteLine()
    {
        sb.AppendLine();
        return this;
    }

    public override string ToString()
    {
        while (Indentation > 0)
        {
            Dedent();
        }
        Debug.Assert(Indentation == 0 && sb.Length > 0);
        return sb.ToString();
    }

    private static ReadOnlySpan<char> GetNextLine(ref ReadOnlySpan<char> remainingText, out bool isFinalLine)
    {
        if (remainingText.IsEmpty)
        {
            isFinalLine = true;
            return default;
        }

        ReadOnlySpan<char> rest;

        int lineLength = remainingText.IndexOf('\n');
        if (lineLength == -1)
        {
            lineLength = remainingText.Length;
            isFinalLine = true;
            rest = default;
        }
        else
        {
            rest = remainingText.Slice(lineLength + 1);
            isFinalLine = false;
        }

        if ((uint)lineLength > 0 && remainingText[lineLength - 1] == '\r')
        {
            lineLength--;
        }

        var next = remainingText.Slice(0, lineLength);
        remainingText = rest;
        return next;
    }
}

file static class StringBuilderExtensions
{
    private const char IndentationChar = ' ';
    private const int CharsPerIndentation = 4;
    public static unsafe StringBuilder AppendSpan(this StringBuilder builder, ReadOnlySpan<char> span)
    {
        fixed (char* ptr = span)
        {
            builder.Append(ptr, span.Length);
        }

        return builder;
    }

    public static StringBuilder AddIndentation(this StringBuilder sb, int indentation) => sb.Append(IndentationChar, CharsPerIndentation * indentation);
}