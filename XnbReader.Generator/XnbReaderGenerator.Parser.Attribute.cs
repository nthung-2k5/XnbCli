using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EnumsNET;
using Microsoft.CodeAnalysis;
using XnbReader.Generator.Helpers;
using XnbReader.Generator.Model;

namespace XnbReader.Generator;

public sealed partial class XnbReaderGenerator
{
    private sealed partial class Parser
    {
        private void ParseXnbReadableAttributes(ISymbol readerClassSymbol, out List<TypeToGenerate>? rootReadableTypes)
        {
            rootReadableTypes = null;

            foreach (var attributeData in readerClassSymbol.GetAttributes())
            {
                var attributeClass = attributeData.AttributeClass;

                if (!SymbolEqualityComparer.Default.Equals(attributeClass, knownSymbols.XnbReadableAttributeType))
                {
                    continue;
                }

                var typeToGenerate = ParseXnbReadableAttribute(attributeData);
                
                if (typeToGenerate is null)
                {
                    continue;
                }

                (rootReadableTypes ??= []).Add(typeToGenerate.Value);
            }
        }
            
        private static TypeToGenerate? ParseXnbReadableAttribute(AttributeData attributeData)
        {
            Debug.Assert(attributeData.ConstructorArguments.Length == 1);
            
            var typeSymbol = (INamedTypeSymbol?)attributeData.ConstructorArguments[0].Value;

            if (typeSymbol is null)
            {
                return null;
            }

            bool reflectiveReader = false;
            string? readerFormat = null;

            foreach (var namedArg in attributeData.NamedArguments)
            {
                object obj = namedArg.Value.Value!;
                switch (namedArg.Key)
                {
                    case "Reflective":
                        reflectiveReader = (bool)obj;
                        break;
                    case "ReaderOverride":
                        readerFormat = (string?)obj;
                        break;
                }
            }

            readerFormat ??= CreateReaderFormat(typeSymbol, reflectiveReader);

            return new TypeToGenerate(typeSymbol, readerFormat);
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

        private static string CreateReaderFormat(INamedTypeSymbol symbol, bool reflective)
        {
            var format = new StringBuilder(ConstStrings.XnaFrameworkContentNamespace);
            format.Append('.');

            if (!reflective)
            {
                format.Append(symbol.Name).Append("Reader");

                if (symbol.IsGenericType)
                {
                    format.Append('`').Append(symbol.Arity).Append('[')
                          .Append(string.Join(",", symbol.TypeArguments.Select(GetFullName)))
                          .Append(']');
                }
            }
            else
            {
                format.Append("ReflectiveReader`1[")
                      .Append(GetFullName(symbol))
                      .Append(']');
            }
            
            return format.ToString();
            
            static string GetFullName(ITypeSymbol symbol)
            {
                return IsBuiltInSupportType(symbol) ? symbol.SpecialType.AsString().Replace('_', '.') : symbol.GetFullyQualifiedName().Substring("global::".Length);
            }
        }
    }
}
