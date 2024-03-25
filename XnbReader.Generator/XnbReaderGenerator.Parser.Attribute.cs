using System.Collections.Generic;
using System.Diagnostics;
using EnumsNET;
using Microsoft.CodeAnalysis;
using XnbReader.Generator.Helpers;
using XnbReader.Generator.Model;

namespace XnbReader.Generator;

public sealed partial class XnbReaderGenerator
{
    private sealed partial class Parser
    {
        private void ParseClassReaderAttributes(ISymbol readerClassSymbol, out List<TypeToGenerate>? rootReadableTypes)
        {
            rootReadableTypes = null;

            foreach (var attributeData in readerClassSymbol.GetAttributes())
            {
                var attributeClass = attributeData.AttributeClass;

                if (!SymbolEqualityComparer.Default.Equals(attributeClass, knownSymbols.XnbReadableAttributeType))
                {
                    continue;
                }

                var typeToGenerate = ParseClassReaderAttribute(attributeData);
                
                if (typeToGenerate is null)
                {
                    continue;
                }

                if (typeToGenerate.Value.TypeReader.HasAllFlags(ContentTypeReader.Default | ContentTypeReader.Reflective))
                {
                    ReportDiagnostic(DiagnosticDescriptors.MultipleSingleReader, attributeData.GetLocation(), attributeData.AttributeClass.Name);
                    continue;
                }

                (rootReadableTypes ??= []).Add(typeToGenerate.Value);
            }
        }
            
        private static TypeToGenerate? ParseClassReaderAttribute(AttributeData attributeData)
        {
            Debug.Assert(attributeData.ConstructorArguments.Length == 1);
            
            var typeSymbol = (ITypeSymbol?)attributeData.ConstructorArguments[0].Value;

            if (typeSymbol is null)
            {
                return null;
            }

            var typeReader = ContentTypeReader.StringKeyDictionary;
            string? readerFormat = null;

            foreach (var namedArg in attributeData.NamedArguments)
            {
                object obj = namedArg.Value.Value!;
                switch (namedArg.Key)
                {
                    case "TypeReader":
                        typeReader = (ContentTypeReader)obj;
                        break;
                    
                    case "ReaderOverride":
                        readerFormat = (string?)obj;
                        break;
                }
            }

            if (typeReader.HasAllFlags(ContentTypeReader.Reflective))
            {
                readerFormat = ConstStrings.ReflectiveReader;
            }
            else if (typeReader.HasAllFlags(ContentTypeReader.Default))
            {
                readerFormat = ConstStrings.DefaultReader;
            }

            return new TypeToGenerate(typeSymbol, typeReader, readerFormat);
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
