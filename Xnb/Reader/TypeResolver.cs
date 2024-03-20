using System.Text;
using System.Text.RegularExpressions;
using Serilog;

namespace Xnb.Reader;

public static partial class TypeResolver
{
    private static readonly Regex TypeSplit = TypeSplitRegex();

    private static readonly Regex TypeInfoSplit = TypeInfoSplitRegex();

    public static bool IsSubclassOfRawGeneric(this Type toCheck, Type baseType)
    {
        while (toCheck is not null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;

            if (baseType == cur)
            {
                return true;
            }

            toCheck = toCheck.BaseType;
        }

        return false;
    }

    public static string SimplifyType(string type)
    {
        var parsed = ParseType(type);
        string simple = parsed.Type.Split(',')[0];

        Log.Debug("Type: {simple}", simple);

        // switch over the possible types registered with this tool
        // switch (simple)
        // {
        //     // Boolean
        //     case "Microsoft.Xna.Framework.Content.BooleanReader":
        //     case "System.Boolean":
        //         return "Boolean";
        //
        //     // Char
        //     case "Microsoft.Xna.Framework.Content.CharReader":
        //     case "System.Char":
        //         return "Char";
        //
        //     // Int32
        //     case "Microsoft.Xna.Framework.Content.Int32Reader":
        //     case "System.Int32":
        //         return "Int32";
        //
        //     // Single
        //     case "Microsoft.Xna.Framework.Content.SingleReader":
        //         return "Single";
        //
        //     // String
        //     case "Microsoft.Xna.Framework.Content.StringReader":
        //     case "System.String":
        //         return "String";
        //
        //     // Dictionary
        //     case "Microsoft.Xna.Framework.Content.DictionaryReader":
        //         return $"Dictionary<{ParseSubtypes(type).SelectToString(SimplifyType)}>";
        //
        //     // Array
        //     case "Microsoft.Xna.Framework.Content.ArrayReader":
        //         return $"Array<{ParseSubtypes(type).SelectToString(SimplifyType)}>";
        //
        //     // List
        //     case "Microsoft.Xna.Framework.Content.ListReader":
        //     case "System.Collections.Generic.List":
        //         return $"List<{ParseSubtypes(type).SelectToString(SimplifyType)}>";
        //
        //     // Texture2D
        //     case "Microsoft.Xna.Framework.Content.Texture2DReader":
        //         return "Texture2D";
        //
        //     // Vector2
        //     case "Microsoft.Xna.Framework.Content.Vector2Reader":
        //     case "Microsoft.Xna.Framework.Vector2":
        //         return "Vector2";
        //
        //     // Vector3
        //     case "Microsoft.Xna.Framework.Content.Vector3Reader":
        //     case "Microsoft.Xna.Framework.Vector3":
        //         return "Vector3";
        //
        //     // Vector3
        //     case "Microsoft.Xna.Framework.Content.Vector4Reader":
        //     case "Microsoft.Xna.Framework.Vector4":
        //         return "Vector4";
        //
        //     // SpriteFont
        //     case "Microsoft.Xna.Framework.Content.SpriteFontReader":
        //         return "SpriteFont";
        //
        //     // Rectangle
        //     case "Microsoft.Xna.Framework.Content.RectangleReader":
        //     case "Microsoft.Xna.Framework.Rectangle":
        //         return "Rectangle";
        //
        //     // Effect
        //     case "Microsoft.Xna.Framework.Content.EffectReader":
        //     case "Microsoft.Xna.Framework.Graphics.Effect":
        //         return "Effect";
        //
        //     // xTile TBin
        //     case "xTile.Pipeline.TideReader":
        //         return "TBin";
        //
        //     // BmFont
        //     case "BmFont.XmlSourceReader":
        //         return "BmFont";
        //
        //     // Reflective
        //     case "Microsoft.Xna.Framework.Content.ReflectiveReader":
        //         return ParseSubtypes(type).SelectToString(SimplifyType);
        //
        //     // MovieData
        //     case "StardewValley.GameData.Movies.MovieData":
        //         return "Reflective<MovieData>";
        //
        //     // MovieSceneReader
        //     case "StardewValley.GameData.Movies.MovieScene":
        //         return "Reflective<MovieScene>";
        //
        //     // ConcessionItemData
        //     case "StardewValley.GameData.Movies.ConcessionItemData":
        //         return "Reflective<ConcessionItemData>";
        //
        //     // ConcessionTaste
        //     case "StardewValley.GameData.Movies.ConcessionTaste":
        //         return "Reflective<ConcessionTaste>";
        //
        //     // FishPondData
        //     case "StardewValley.GameData.FishPond.FishPondData":
        //         return "Reflective<FishPondData>";
        //
        //     // FishPondReward
        //     case "StardewValley.GameData.FishPond.FishPondReward":
        //         return "Reflective<FishPondReward>";
        //
        //     // MovieCharacterReaction
        //     case "StardewValley.GameData.Movies.MovieCharacterReaction":
        //         return "Reflective<MovieCharacterReaction>";
        //
        //     // MovieReaction
        //     case "StardewValley.GameData.Movies.MovieReaction":
        //         return "Reflective<MovieReaction>";
        //
        //     // SpecialResponses
        //     case "StardewValley.GameData.Movies.SpecialResponses":
        //         return "Reflective<SpecialResponses>";
        //
        //     // CharacterResponse
        //     case "StardewValley.GameData.Movies.CharacterResponse":
        //         return "Reflective<CharacterResponse>";
        //
        //     // TailorItemRecipe
        //     case "StardewValley.GameData.Crafting.TailorItemRecipe":
        //         return "Reflective<TailorItemRecipe>";
        //
        //     // unimplemented type catch
        //     default:
        //         throw new XnbException($"Non-implemented type found, cannot resolve type \"{simple}\", \"${type}\".");
        // }
        /*  curent main readers:
            BmFont.XmlSourceReader
            xTile.Pipeline.TideReader
            Microsoft.Xna.Framework.Content.EffectReader
            Microsoft.Xna.Framework.Content.SpriteFontReader
            Microsoft.Xna.Framework.Content.Texture2DReader

            Dictionary<int, string>
            List<string>
            Dictionary<string, string>

            List<TailorItemRecipe>
            List<MovieCharacterReaction>
            List<ConcessionTaste>
            List<FishPondData>
            List<ConcessionItemData>
            Dictionary<string, MovieData>
        */

        StringBuilder fullType = new(simple);

        if (parsed.GenericArgs is not null)
        {
            fullType.Append($"`{parsed.GenericArgs.Length}[").AppendJoin(',', parsed.GenericArgs.Select(generic => generic.Replace("StardewValley.GameData", "Xnb.Types.StardewValley"))).Append(']');
        }

        return fullType.ToString();
    }

    private static (string Type, string[] GenericArgs) ParseType(string type)
    {
        if (!type.Contains('`'))
        {
            return (type, null);
        }

        var res = TypeSplit.Match(type);

        string tName = res.Groups["TypeName"].Value;
        string genericArgs = res.Groups["GenericArgs"].Value;

        return (tName, ParseSubtypes(genericArgs).ToArray());
    }

    private static IEnumerable<string> ParseSubtypes(string types)
    {
        var res = TypeInfoSplit.Matches(types);
        return res.Select(val => val.Groups[1].Value);
    }

    [GeneratedRegex(@"(?<TypeName>.*?)`\d*\[(?<GenericArgs>.*)\]")]
    private static partial Regex TypeSplitRegex();

    [GeneratedRegex(@"\[([^\[\]]*?),(?:[^\[\]]*)\]")]
    private static partial Regex TypeInfoSplitRegex();
}
