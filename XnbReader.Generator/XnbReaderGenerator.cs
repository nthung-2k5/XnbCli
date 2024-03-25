// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using XnbReader.Generator.Helpers;
using XnbReader.Generator.Immutable;
using XnbReader.Generator.Model;

namespace XnbReader.Generator;

/// <summary>
/// Generates source code to optimize serialization and deserialization with XnbSerializer.
/// </summary>
[Generator]
public sealed partial class XnbReaderGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var knownTypeSymbols = context.CompilationProvider.Select((compilation, _) => new KnownTypeSymbols(compilation));

        var readerGenerationSpecs = context.SyntaxProvider
                                           .ForAttributeWithMetadataName(
                                               ConstStrings.FullXnbReadableAttribute,
                                               static (node, _) => node is ClassDeclarationSyntax or RecordDeclarationSyntax,
                                               static (ctx, _) => (ReaderClass: (ClassDeclarationSyntax)ctx.TargetNode, ctx.SemanticModel))
                                           .Combine(knownTypeSymbols)
                                           .Select(static (tuple, cancellationToken) =>
                                           {
                                               var parser = new Parser(tuple.Right);
                                               var readerGenerationSpec = parser.ParseReaderGenerationSpec(tuple.Left.ReaderClass, tuple.Left.SemanticModel, cancellationToken);
                                               var diagnostics = parser.Diagnostics.ToImmutableEquatableArray();
                                               return (GenerationSpec: readerGenerationSpec, Diagnostics: diagnostics);
                                           });
        
        context.RegisterSourceOutput(readerGenerationSpecs, ReportDiagnosticsAndEmitSource);
    }

    private static void ReportDiagnosticsAndEmitSource(SourceProductionContext sourceProductionContext, (ReaderGenerationSpec GenerationSpec, ImmutableEquatableArray<Diagnostic> Diagnostics) input)
    {
        // Report any diagnostics ahead of emitting.
        foreach (var diagnostic in input.Diagnostics)
        {
            sourceProductionContext.ReportDiagnostic(diagnostic);
        }

        if (input.GenerationSpec is null)
        {
            return;
        }
    
        Emitter emitter = new(sourceProductionContext);
        emitter.Emit(input.GenerationSpec);
    }
    
    private partial class Emitter(SourceProductionContext context)
    {
        private partial void AddSource(string hintName, SourceText sourceText) => context.AddSource(hintName, sourceText);
    }
}