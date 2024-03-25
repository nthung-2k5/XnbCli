// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Mono.Cecil;
using XnbCli.CecilPorter;

Debug.Assert(args.Length == 2);
(string assemblyFolder, string outputCodeFolder) = (args[0], args[1]);

using var stardewGameData = AssemblyDefinition.ReadAssembly(Path.Combine(assemblyFolder, "StardewValley.GameData.dll"));
using var module = stardewGameData.MainModule;

CodeGenerator generator = new(outputCodeFolder);

Dictionary<string, TypeData> types = new();

foreach (var type in module.Types)
{
    ProcessType(type);
}

foreach (var type in types.Values)
{
    generator.Export(type);
}

return;

TypeData ProcessType(TypeDefinition type, bool exportAsFile = true)
{
    if (types.TryGetValue(type.FullName, out var processType))
    {
        return processType;
    }

    string ns = string.Join('.', type.Namespace.Split('.').Where(ns => ns != "StardewValley" && ns != "GameData"));
    string[] interfaces = type.Interfaces.Select(inter => inter.InterfaceType.FullName.Replace("StardewValley.GameData", "XnbReader.StardewValley")).ToArray();

    TypeData? data = null;
    
    if (type.IsEnum)
    {
        string[] members = type.Fields.Skip(1).Select(field => field.Name).ToArray();
        data = new EnumData(ns, type.Name, members);
    }
    else if (type.IsInterface)
    {
        var members = type.Properties.Select(field => new MemberData(field.PropertyType.FullName, field.Name)).ToArray();
        data = new InterfaceData(ns, type.Name, members, interfaces);
    }
    else if (type.IsClass && type.Name != "<Module>")
    {
        var members = Enumerable.Empty<MemberData>();
        ClassData? baseType = null;
        if (!type.BaseType.FullName.Contains("System.Object"))
        {
            baseType = ProcessType((TypeDefinition)type.BaseType) as ClassData;
            members = baseType!.Members;
        }
        
        members = members.Concat(GetValidMembers(type).Select(member =>
        {
            string memberType = member switch
            {
                PropertyDefinition prop => prop.PropertyType.FullName,
                FieldDefinition field => field.FieldType.FullName,
                _ => throw new NotSupportedException()
            };
            return new MemberData(memberType, member.Name);
        }));

        var nestedTypes = type.NestedTypes.Select(nested => ProcessType(nested, false)).ToArray();
        data = new ClassData(ns, type.Name, members.ToArray(), baseType, interfaces, nestedTypes);
    }
    
    if (exportAsFile && data is not null)
    {
        types[type.FullName] = data;
    }

    return data!;
}

IEnumerable<MemberReference> GetValidMembers(TypeDefinition type)
{
    var validProps = type.Properties.Where(ValidMemberPredicate.IsValidProperty);
    var validFields = type.Fields.Where(ValidMemberPredicate.IsValidField);
    return validProps.Concat<MemberReference>(validFields);
}