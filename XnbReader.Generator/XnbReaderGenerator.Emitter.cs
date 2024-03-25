// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using XnbReader.Generator.Helpers;
using XnbReader.Generator.Model;

namespace XnbReader.Generator;

public sealed partial class XnbReaderGenerator
{
    private sealed partial class Emitter
    {
        // Literals in generated source
        private const string PropertyPrefix = "Prop_";

        // global::fully.qualified.name for referenced types
        private const string NotImplementedExceptionTypeRef = "global::System.NotImplementedException";
        private const string UnsafeTypeRef = "global::System.Runtime.CompilerServices.Unsafe";
        private const string MemoryMarshalTypeRef = "global::System.Runtime.InteropServices.MemoryMarshal";
        private const string XnbStreamTypeRef = "global::XnbReader.XnbStream";
        private const string GarbageCollectorTypeRef = "global::System.GC";
        private const string ListTypeRef = "global::System.Collections.Generic.List";
        private const string DictionaryTypeRef = "global::System.Collections.Generic.Dictionary";
        private const string MemoryOwnerTypeRef = "global::CommunityToolkit.HighPerformance.Buffers.MemoryOwner";
        
        /// <summary>
        /// Contains an index from TypeRef to TypeGenerationSpec for the current ReaderGenerationSpec.
        /// </summary>
        private readonly Dictionary<TypeRef, TypeGenerationSpec> typeIndex = new();

        /// <summary>
        /// The SourceText emit implementation filled by the individual Roslyn versions.
        /// </summary>
        private partial void AddSource(string hintName, SourceText sourceText);

        public void Emit(ReaderGenerationSpec readerGenerationSpec)
        {
            Debug.Assert(typeIndex.Count == 0);

            foreach (var spec in readerGenerationSpec.GeneratedTypes)
            {
                typeIndex.Add(spec.TypeRef, spec);
            }

            foreach (var typeGenerationSpec in readerGenerationSpec.GeneratedTypes)
            {
                var sourceText = GenerateTypeInfo(readerGenerationSpec, typeGenerationSpec);
                if (sourceText != null)
                {
                    AddSource($"{readerGenerationSpec.ReaderType.Name}.{typeGenerationSpec.TypeRef.TypeInfoName}.g.cs", sourceText);
                }
            }

            string readerName = readerGenerationSpec.ReaderType.Name;

            // Add root reader implementation.
            AddSource($"{readerName}.g.cs", GetRootClassReaderImplementation(readerGenerationSpec));
            AddSource($"{readerName}.Constructor.g.cs", GetAsIsConstructor(readerGenerationSpec));
            typeIndex.Clear();
        }

        private static SourceText? GenerateTypeInfo(ReaderGenerationSpec readerSpec, TypeGenerationSpec typeGenerationSpec)
        {
            switch (typeGenerationSpec.ClassType)
            {
                case ClassType.Nullable:
                    return GenerateForNullable(readerSpec, typeGenerationSpec);

                case ClassType.Enum:
                    return GenerateForEnum(readerSpec, typeGenerationSpec);

                case ClassType.Enumerable:
                case ClassType.Dictionary:
                    return GenerateForCollection(readerSpec, typeGenerationSpec);

                case ClassType.Object:
                    return GenerateForObject(readerSpec, typeGenerationSpec);
                
                case ClassType.BuiltInSupportType:
                    return null; // Do not emit a source file for the type.
                
                case ClassType.Unmanaged:
                    return GenerateForUnmanaged(readerSpec, typeGenerationSpec);
                
                default:
                    Debug.Fail($"Unexpected class type {typeGenerationSpec.ClassType}");
                    return null;
            }
        }

        private static SourceText GenerateForNullable(ReaderGenerationSpec readerSpec, TypeGenerationSpec typeMetadata)
        {
            Debug.Assert(typeMetadata.UnderlyingType != null);

            return readerSpec.CreateSourceWriterWithReaderHeader()
                             .GenerateTypeInfoFactoryHeader(typeMetadata)
                             .WriteLine($"return ReadBoolean() ? Read{typeMetadata.UnderlyingType.Name}() : default;")
                             .CompleteSourceFileAndReturnText();
        }

         private static SourceText GenerateForEnum(ReaderGenerationSpec readerSpec, TypeGenerationSpec typeMetadata)
         {
             Debug.Assert(typeMetadata.UnderlyingType != null);
             return readerSpec.CreateSourceWriterWithReaderHeader()
                              .GenerateTypeInfoFactoryHeader(typeMetadata)
                              .WriteLine($"return ({typeMetadata.TypeRef.FullyQualifiedName})Read{typeMetadata.UnderlyingType.Name}();")
                              .CompleteSourceFileAndReturnText();
         }

        private static SourceText GenerateForCollection(ReaderGenerationSpec readerSpec, TypeGenerationSpec typeGenerationSpec)
        {
            // Value metadata
            var collectionValueType = typeGenerationSpec.CollectionValueType;
            Debug.Assert(collectionValueType != null);
            string valueTypeName = collectionValueType.TypeInfoName;
            string valueTypeFullName = collectionValueType.FullyQualifiedName;

            var collectionType = typeGenerationSpec.CollectionType;
            string createCollectionMethodExpr;

            switch (collectionType)
            {
                case CollectionType.Array when collectionValueType.IsUnmanagedType:
                    createCollectionMethodExpr = $"""
                                                  int length = ReadInt32();
                                                  
                                                  var array = {GarbageCollectorTypeRef}.AllocateUninitializedArray<{valueTypeFullName}>(length);
                                                  _ = Read({(collectionValueType.Name != "Char" ? $"{MemoryMarshalTypeRef}.AsBytes(array.AsSpan())" : "array.AsSpan()")});
                                                  
                                                  return array;
                                                  """;
                    break;
                case CollectionType.Array:
                    createCollectionMethodExpr = $$"""
                                                   int len = ReadInt32();
                                                   var array = new T[len];
                                                   
                                                   for (int i = 0; i < len; i++)
                                                   {
                                                       array[i] = {{CheckNullReader(collectionValueType, $"Read{valueTypeName}()")}};
                                                   }
                                                   
                                                   return array;
                                                   """;
                    break;
                case CollectionType.MemoryOwnerOfT:
                    Debug.Assert(collectionValueType.IsValueType);
                    createCollectionMethodExpr = $"""
                                                  int length = ReadInt32();
                                                  var memory = {MemoryOwnerTypeRef}<{valueTypeFullName}>.Allocate(length);
                                                  
                                                  var span = {MemoryMarshalTypeRef}.AsBytes(memory.Span);
                                                  _ = Read(span);
                                                  
                                                  return memory;
                                                  """;
                    break;
                case CollectionType.Dictionary:
                    // Key metadata
                    var collectionKeyType = typeGenerationSpec.CollectionKeyType;
                    Debug.Assert(collectionKeyType != null);
                    string? keyTypeName = collectionKeyType?.TypeInfoName;
                    string? keyTypeFullName = collectionKeyType?.FullyQualifiedName;
                    createCollectionMethodExpr = $$"""
                                                   int len = ReadInt32();
                                                   var dict = new {{DictionaryTypeRef}}<{{keyTypeFullName}}, {{valueTypeFullName}}>(len);
                                                   
                                                   for (int i = 0; i < len; i++)
                                                   {
                                                       var key = {{CheckNullReader(collectionKeyType, $"Read{keyTypeName}()")}};
                                                       var value = {{CheckNullReader(collectionValueType, $"Read{valueTypeName}()")}};
                                                       dict.Add(key, value);
                                                   }
                                                   
                                                   return dict;
                                                   """;
                    break;
                case CollectionType.List:
                    createCollectionMethodExpr = $$"""
                                                   int len = ReadInt32();
                                                   var array = new {{ListTypeRef}}<{{valueTypeFullName}}>(len);
                                                   
                                                   for (int i = 0; i < len; i++)
                                                   {
                                                       array.Add({{CheckNullReader(collectionValueType, $"Read{valueTypeName}()")}});
                                                   }
                                                   
                                                   return array;
                                                   """;
                    break;
                case CollectionType.Unsupported:
                case CollectionType.MultiArray:
                default:
                    throw new NotSupportedException();
            }

            return readerSpec.CreateSourceWriterWithReaderHeader()
                             .GenerateTypeInfoFactoryHeader(typeGenerationSpec)
                             .WriteLine(createCollectionMethodExpr)
                             .CompleteSourceFileAndReturnText();
        }

        private static SourceText GenerateForObject(ReaderGenerationSpec readerSpec, TypeGenerationSpec typeMetadata)
        {
            var writer = readerSpec.CreateSourceWriterWithReaderHeader()
                                   .GenerateTypeInfoFactoryHeader(typeMetadata);

            if (!typeMetadata.ImplementsICustomReader)
            {
                GenerateReadProperty(writer, typeMetadata);
                GenerateCtor(writer, typeMetadata);
            }
            else
            {
                GenerateStaticCustomReaderInvocation(writer, typeMetadata);
            }

            return writer.CompleteSourceFileAndReturnText();
        }

        private static void GenerateReadProperty(SourceWriter writer, TypeGenerationSpec typeGenerationSpec)
        {
            var properties = typeGenerationSpec.PropertyGenSpecs;

            if (typeGenerationSpec.ConstructionStrategy == ObjectConstructionStrategy.ParameterizedConstructor)
            {
                foreach (var parameter in typeGenerationSpec.CtorParamGenSpecs)
                {
                    string parameterName = parameter.Name;
                    string parameterTypeName = parameter.ParameterType.TypeInfoName;

                    writer.WriteLine($"var {PropertyPrefix}{parameterName} = {CheckNullReader(parameter.HasHeader ?? parameter.ParameterType is { IsValueType: false, IsDiscardReaderType: false }, $"Read{parameterTypeName}()")};");
                }
            }
            else
            {
                foreach (var property in properties)
                {
                    string propertyName = property.MemberName;
                    string propertyTypeName = property.PropertyType.TypeInfoName;
                    
                    writer.WriteLine($"var {PropertyPrefix}{propertyName} = {CheckNullReader(property.HasHeader ?? property.PropertyType is { IsValueType: false, IsDiscardReaderType: false }, $"Read{propertyTypeName}()")};");
                }
            }
        }

        private static void GenerateCtor(SourceWriter writer, TypeGenerationSpec typeGenerationSpec)
        {
            string typeName = typeGenerationSpec.TypeRef.FullyQualifiedName;
            var parameters = typeGenerationSpec.CtorParamGenSpecs;
            var propertyInitializers = typeGenerationSpec.PropertyGenSpecs;
            
            writer.AddIndentation().Write($"return new {typeName}({string.Join(", ", parameters.Select(spec => PropertyPrefix + spec.Name))})");

            if (propertyInitializers.Count > 0 && typeGenerationSpec.ConstructionStrategy == ObjectConstructionStrategy.ParameterlessConstructor)
            {
                writer.Indent();
            
                foreach (var spec in propertyInitializers)
                {
                    writer.WriteLine($"{spec.MemberName} = {PropertyPrefix}{spec.MemberName},");
                }

                writer.Dedent().Write(';').WriteLine();
            }
            else
            {
                writer.Write(';').WriteLine();
            }
        }
        
        private static void GenerateStaticCustomReaderInvocation(SourceWriter writer, TypeGenerationSpec typeGenerationSpec)
        {
            writer.WriteLine($"return {typeGenerationSpec.TypeRef.FullyQualifiedName}.Read(this);");
        }
        
        private static SourceText GenerateForUnmanaged(ReaderGenerationSpec readerSpec, TypeGenerationSpec typeMetadata)
        {
            return readerSpec.CreateSourceWriterWithReaderHeader()
                             .GenerateTypeInfoFactoryHeader(typeMetadata)
                             .WriteLine($"return {UnsafeTypeRef}.ReadUnaligned<{typeMetadata.TypeRef.FullyQualifiedName}>(ref ReadBytes({UnsafeTypeRef}.SizeOf<{typeMetadata.TypeRef.FullyQualifiedName}>())[0]);")
                             .CompleteSourceFileAndReturnText();
        }

        private static SourceText GetRootClassReaderImplementation(ReaderGenerationSpec readerSpec)
        {
            var writer = readerSpec.CreateSourceWriterWithReaderHeader()
                                   .WriteLine("public override object Read(string readerType)")
                                   .Indent()
                                   .WriteLine("_ = Read7BitEncodedInt(); // discard reader, because we know what the type is")
                                   .WriteLine("switch (readerType)")
                                   .Indent();

            foreach (var topType in readerSpec.RootLevelTypes)
            {
                writer.WriteLine($"""
                                  case {SymbolDisplay.FormatLiteral(topType.ToString(), true)}:
                                      return {topType.CreateTypeInfoMethodName()}();
                                  """);
            }
            
            return writer.WriteLine($"""
                                     default:
                                         throw new {NotImplementedExceptionTypeRef}();
                                     """)
                         .CompleteSourceFileAndReturnText();
        }

        private static SourceText GetAsIsConstructor(ReaderGenerationSpec readerSpec)
        {
            string readerClass = readerSpec.ReaderType.Name;
            return readerSpec.CreateSourceWriterWithReaderHeader(isPrimaryReaderSourceFile: true)
                             .WriteLine($"public {readerClass}({XnbStreamTypeRef} input) : base(input) {{ }}")
                             .CompleteSourceFileAndReturnText();
        }
        
        private static string CheckNullReader([NotNull] TypeRef? typeRef, string body)
        {
            return CheckNullReader(!typeRef.IsValueType, body);
        }
        private static string CheckNullReader(bool condition, string body)
        {
            return condition ? $"Read7BitEncodedInt() != 0 ? {body} : default" : body;
        }
    }
}

file static class SourceWriterExtensions
{
    private static readonly AssemblyName AssemblyName = typeof(XnbReaderGenerator).Assembly.GetName();
    
    public static SourceWriter CreateSourceWriterWithReaderHeader(this ReaderGenerationSpec readerSpec, bool isPrimaryReaderSourceFile = false, string? interfaceImplementation = null)
    {
        var writer = new SourceWriter();

        writer.WriteLine("""
                         // <auto-generated/>

                         #nullable enable annotations
                         #nullable disable warnings

                         """);

        if (readerSpec.Namespace != null)
        {
            writer.WriteLine($"namespace {readerSpec.Namespace}").Indent();
        }

        var readerClasses = readerSpec.ReaderClassDeclarations;
        Debug.Assert(readerClasses.Count > 0);

        // Emit any containing classes first.
        for (int i = readerClasses.Count - 1; i > 0; i--)
        {
            writer.WriteLine(readerClasses[i]).Indent();
        }

        if (isPrimaryReaderSourceFile)
        {
            // Annotate reader class with the GeneratedCodeAttribute
            writer.WriteLine($"""[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{AssemblyName.Name}", "{AssemblyName.Version}")]""");
        }

        // Emit the ClassReaders class declaration
        return writer.WriteLine($"{readerClasses[0]}{(interfaceImplementation is null ? "" : " : " + interfaceImplementation)}").Indent();
    }
    
    public static SourceWriter GenerateTypeInfoFactoryHeader(this SourceWriter writer, TypeGenerationSpec typeMetadata)
    {
        return writer.WriteLine($"protected {typeMetadata.TypeRef.FullyQualifiedName} {CreateTypeInfoMethodName(typeMetadata)}()").Indent();
    }
    
    /// <summary>
    /// Method used to generate Read method specifically for a type info
    /// </summary>
    public static string CreateTypeInfoMethodName(this TypeGenerationSpec typeSpec) => $"Read{typeSpec.TypeRef.TypeInfoName}";
}