using StbImageWriteSharp;

namespace XnbReader.Texture;

public static class ImageWriterExtensions
{
    public static void Write(this ImageWriter writer, Texture2D texture, Stream stream)
    {
        unsafe
        {
            using var memoryHandle = texture.Data.Pin();
            writer.WritePng(memoryHandle.Pointer, texture.Width, texture.Height, ColorComponents.RedGreenBlueAlpha, stream);
        }
    }
}
