using Xnb.Types;

namespace Xnb.Reader.ContentReader
{
    public abstract class ExplicitReader<T, U>: BaseReader<T> where T: ExplicitType<U>, new()
    {
        public override T Read(BinaryReader buffer)
        {
            return new T { Data = ReaderResolver.Read<U>(buffer) };
        }

        public override void Write(BinaryWriter buffer, T content)
        {
            ReaderResolver.Write(buffer, content.Data);
        }
    }

    public static partial class ReaderResolver
    {
        static partial void RegisterExplicitReaders()
        {
                Register(new TBinReader());
                Register(new EffectReader());
                Register(new BmFontReader());
            Register(new Texture2DReader());
            Register(new SpriteFontReader());
        }
    }
    public class TBinReader: ExplicitReader<TBin, byte[]>;
    public class EffectReader: ExplicitReader<Effect, byte[]>;
    public class BmFontReader: BaseReader<BmFont>
    {
        public override BmFont Read(BinaryReader buffer)
        {
            return new BmFont { Data = buffer.ReadString() };
        }

        public override void Write(BinaryWriter buffer, BmFont content)
        {
            buffer.Write(content.Data);
        }
    }
}

namespace Xnb.Types
{
    public record TBin: ExplicitType<byte[]>;
    public record Effect: ExplicitType<byte[]>;
    public record BmFont: ExplicitType<string>;
}

