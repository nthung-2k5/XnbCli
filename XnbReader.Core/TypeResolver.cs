using System.Text;
using System.Text.RegularExpressions;
using Serilog;

namespace XnbReader;

public partial class TypeResolver
{
    private static readonly Regex TypeSplit = TypeSplitRegex();

    private static readonly Regex TypeInfoSplit = TypeInfoSplitRegex();

    public static readonly TypeResolver Default = new();

    public virtual string SimplifyType(string type)
    {
        var parsed = ParseType(type);
        string simple = parsed.Type.Split(',')[0];

        Log.Debug("Type: {simple}", simple);

        StringBuilder fullType = new(simple);

        if (parsed.GenericArgs is not null)
        {
            fullType.Append($"`{parsed.GenericArgs.Length}[").AppendJoin(',', parsed.GenericArgs).Append(']');
        }

        return fullType.ToString();
    }

    private static (string Type, string[]? GenericArgs) ParseType(string type)
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
