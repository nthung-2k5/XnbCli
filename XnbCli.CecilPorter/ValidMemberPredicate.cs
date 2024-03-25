using Mono.Cecil;

namespace XnbCli.CecilPorter;

public static class ValidMemberPredicate
{
    private const string ContentSerializerFullName = "Microsoft.Xna.Framework.Content.ContentSerializerAttribute";
    private const string ContentSerializerIgnoreFullName = "Microsoft.Xna.Framework.Content.ContentSerializerIgnoreAttribute";
    
    public static bool IsValidProperty(PropertyDefinition property)
    {
        if (property.GetMethod is null || property.Parameters.Count != 0)
        {
            return false;
        }

        if (property.CustomAttributes.ContainsAttribute(ContentSerializerIgnoreFullName))
        {
            return false;
        }

        bool forceSerialize = property.CustomAttributes.ContainsAttribute(ContentSerializerFullName);

        return forceSerialize || (property.GetMethod.Attributes & MethodAttributes.Public) == MethodAttributes.Public;
    }
    
    public static bool IsValidField(FieldDefinition field)
    {
        if (field.CustomAttributes.ContainsAttribute(ContentSerializerIgnoreFullName))
        {
            return false;
        }

        bool forceSerialize = field.CustomAttributes.ContainsAttribute(ContentSerializerFullName);

        return forceSerialize || ((field.Attributes & FieldAttributes.Public) == FieldAttributes.Public && !field.IsInitOnly);
    }

    private static bool ContainsAttribute(this IEnumerable<CustomAttribute> attrs, string name)
    {
        return attrs.Any(attr => attr.AttributeType.FullName == name);
    }
}
