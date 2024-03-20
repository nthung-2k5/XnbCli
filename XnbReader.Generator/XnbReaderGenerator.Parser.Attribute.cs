using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using XnbReader.Generator.Helpers;

namespace XnbReader.Generator;

public sealed partial class XnbReaderGenerator
{
    private sealed partial class Parser
    {
        private void ParseClassReaderAttributes(ISymbol readerClassSymbol, out List<TypeToGenerate>? rootSerializableTypes)
        {
            rootSerializableTypes = null;

            foreach (var attributeData in readerClassSymbol.GetAttributes())
            {
                var attributeClass = attributeData.AttributeClass;

                if (SymbolEqualityComparer.Default.Equals(attributeClass, knownSymbols.ClassReaderAttributeType))
                {
                    var typeToGenerate = ParseClassReaderAttribute(attributeData);
                    if (typeToGenerate is null)
                    {
                        continue;
                    }

                    (rootSerializableTypes ??= []).Add(typeToGenerate.Value);
                }
            }
        }
            
        private static TypeToGenerate? ParseClassReaderAttribute(AttributeData attributeData)
        {
            Debug.Assert(attributeData.ConstructorArguments.Length == 1);
            
            var typeSymbol = (ITypeSymbol?)attributeData.ConstructorArguments[0].Value;
            string? typeFormat = null;
            string? readerFormat = null;

            foreach (var namedArg in attributeData.NamedArguments)
            {
                switch (namedArg.Key)
                {
                    case "NamespaceOverride":
                        typeFormat = (string?)namedArg.Value.Value!;
                        break;
                    
                    case "ReaderOverride":
                        readerFormat = (string?)namedArg.Value.Value!;
                        break;
                }
            }

            if (typeSymbol is null)
            {
                return null;
            }

            return new TypeToGenerate(typeSymbol, typeFormat, readerFormat);
        }
        
        private static void ProcessMemberCustomAttributes(ISymbol memberInfo, out bool? hasHeader)
        {
            Debug.Assert(memberInfo is IFieldSymbol or IPropertySymbol or IParameterSymbol);
            hasHeader = null;

            foreach (var attributeData in memberInfo.GetAttributes())
            {
                var attributeType = attributeData.AttributeClass;

                if (attributeType is null || attributeType.ToDisplayString() != ConstStrings.FullHasHeaderAttribute)
                {
                    continue;
                }

                var ctorArgs = attributeData.ConstructorArguments;
                hasHeader = (bool)ctorArgs[0].Value!;
            }
        }
    }
}
