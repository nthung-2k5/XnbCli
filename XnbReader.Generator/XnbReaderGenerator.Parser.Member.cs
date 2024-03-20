using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using XnbReader.Generator.Model;
using XnbReader.Generator.Helpers;

namespace XnbReader.Generator;

public sealed partial class XnbReaderGenerator
{
    private sealed partial class Parser
    {
        private List<PropertyGenerationSpec> ParsePropertyGenerationSpecs(in TypeToGenerate typeToGenerate)
        {
            List<PropertyGenerationSpec> properties = [];

            // Walk the type hierarchy starting from the current type up to the base type(s)
            foreach (var currentType in typeToGenerate.Type.GetSortedTypeHierarchy())
            {
                var members = currentType.GetMembers();

                foreach (var propertyInfo in members.OfType<IPropertySymbol>())
                {
                    // Skip if property is static or an indexer
                    if (propertyInfo.IsStatic || propertyInfo.Parameters.Length > 0)
                    {
                        continue;
                    }

                    AddMember(memberType: propertyInfo.Type, memberInfo: propertyInfo);
                }

                foreach (var fieldInfo in members.OfType<IFieldSymbol>())
                {
                    // Skip if:
                    if (
                        // it is a static field, constant
                        fieldInfo.IsStatic || fieldInfo.IsConst ||
                        // it is a compiler-generated backing field
                        fieldInfo.AssociatedSymbol != null ||
                        // symbol represents an explicitly named tuple element
                        fieldInfo.IsExplicitlyNamedTupleElement)
                    {
                        continue;
                    }

                    AddMember(memberType: fieldInfo.Type, memberInfo: fieldInfo);
                }
            }

            return properties;

            void AddMember(
                ITypeSymbol memberType,
                ISymbol memberInfo)
            {
                var propertySpec = ParsePropertyGenerationSpec(memberType, memberInfo);

                if (propertySpec is null)
                {
                    // ignored invalid property
                    return;
                }

                properties.Add(propertySpec);
            }
        }

        private PropertyGenerationSpec? ParsePropertyGenerationSpec(
            ITypeSymbol memberType,
            ISymbol memberInfo)
        {
            Debug.Assert(memberInfo is IFieldSymbol or IPropertySymbol);

            ProcessMemberCustomAttributes(memberInfo, out bool? hasHeader);
            
            ProcessMember(memberInfo, out bool canUseSetter);

            if (!canUseSetter)
            {
                // Skip the member if member has no accessible setters
                return null;
            }

            // Enqueue the property type for generation, unless the member is ignored.
            var propertyTypeRef = EnqueueType(memberType, IsDiscardReaderType(memberType));

            return new PropertyGenerationSpec
            {
                MemberName = memberInfo.Name,
                HasHeader = hasHeader,
                PropertyType = propertyTypeRef
            };
        }
        
        private static void ProcessMember(
            ISymbol memberInfo,
            out bool canUseSetter)
        {
            canUseSetter = false;

            switch (memberInfo)
            {
                case IPropertySymbol propertyInfo:
                    if (propertyInfo.SetMethod is { DeclaredAccessibility: Accessibility.Public })
                    {
                        canUseSetter = true;
                    }
                    break;
                case IFieldSymbol fieldInfo:
                    if (fieldInfo.DeclaredAccessibility is Accessibility.Public)
                    {
                        canUseSetter = true;
                    }
                    break;
                default:
                    Debug.Fail("Method given an invalid symbol type.");
                    break;
            }
        }
    }
}
