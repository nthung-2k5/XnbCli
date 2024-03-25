using System.Text;

namespace XnbCli.CecilPorter;

public class CodeGenerator(string folder)
{
    public void Export(TypeData data)
    {
        var directory = Directory.CreateDirectory(Path.Combine(new[] { folder }.Concat(data.Namespace.Split('.')).ToArray()));
        StringBuilder builder = new(data.GetNamespaceSyntax());
        builder.AppendLine().AppendLine().Append(data);
        File.WriteAllText(Path.Combine(directory.FullName, data.Name + ".cs"), builder.ToString());
    }
}
