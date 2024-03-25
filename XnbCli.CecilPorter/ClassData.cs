using System.Text;

namespace XnbCli.CecilPorter;

public record EnumData(string Namespace, string Name, string[] Members) : TypeData(Namespace, Name)
{
    public override string ToString()
    {
        SourceWriter writer = new();
        writer.WriteLine($"public enum {Name}").Indent();

        foreach (string enums in Members)
        {
            writer.WriteLine(enums + ',');
        }

        return writer.ToString();
    }
}

public record ClassData(string Namespace, string Name, MemberData[] Members, ClassData? BaseType, string[] Interfaces, TypeData[] NestedTypes) : TypeData(Namespace, Name)
{
    public override string ToString()
    {
        StringBuilder decl = new("public record ");
        decl.Append(Name)
            .Append('(')
            .AppendJoin(", ", Members.Select(mem => mem.ToString()))
            .Append(')');
        
        if (BaseType is not null || Interfaces.Length > 0)
        {
            decl.Append(": ");
            
            var inherits = new List<string>();
            if (BaseType is not null)
            {
                inherits.Add($"{BaseType.Name}({string.Join(", ", BaseType.Members.Select(mem => mem.Name))})");
            }
        
            inherits.AddRange(Interfaces);

            decl.AppendJoin(", ", inherits);
        }

        if (NestedTypes.Length == 0)
        {
            decl.Append(';');
        }
        
        SourceWriter writer = new();
        writer.WriteLine(decl.ToString());

        if (NestedTypes.Length > 0)
        {
            writer.Indent();
            foreach (var nested in NestedTypes)
            {
                writer.WriteLine(nested.ToString());
            }

            writer.Dedent();
        }
        
        return writer.ToString();
    }
}

public record InterfaceData(string Namespace, string Name, MemberData[] Members, string[] Interfaces): TypeData(Namespace, Name)
{
    public override string ToString()
    {
        string decl = $"public interface {Name}";
        if (Interfaces.Length > 0)
        {
            decl += $": {string.Join(", ", Interfaces)}";
        }
        
        SourceWriter writer = new();
        writer.WriteLine(decl).Indent();

        foreach (var member in Members)
        {
            writer.WriteLine($"public {member} {{ get; init; }}");
        }

        return writer.ToString();
    }
}

public readonly record struct MemberData(string Type, string Name)
{
    public override string ToString() => $"{GetNamespace()} {Name}";

    private string GetNamespace()
    {
        string type = Type.Replace('/', '.').Replace("`1", "").Replace("`2", "");

        if (type.Contains("StardewValley.GameData"))
        {
            type = type.Replace("StardewValley.GameData", "XnbReader.StardewValley");
        }
        else if (type.Contains("StardewValley"))
        {
            type = type.Replace("StardewValley", "XnbReader.StardewValley");
        }

        return type.Replace("Microsoft.Xna.Framework", type.Contains("Vector2") ? "System.Numerics" : "System.Drawing");
    }
}

public record TypeData(string Namespace, string Name)
{
    private const string BaseNamespace = "XnbReader.StardewValley";

    public string GetNamespaceSyntax()
    {
        string ns = BaseNamespace;

        if (!string.IsNullOrEmpty(Namespace))
        {
            ns += $".{Namespace}";
        }

        return $"namespace {ns};";
    }
}
