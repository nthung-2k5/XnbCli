using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using XnbReader.Generator.Model;
using XnbReader.Generator.Helpers;

namespace XnbReader.Generator;

public sealed partial class XnbReaderGenerator
{
    private sealed partial class Parser
    {
        /// <summary>
        /// Validate if the type has the allowed construction method.
        /// There are only 3 construction method, either:
        /// 1) A type with no constructor with no parameter and at least an init-able property (a POCO object).
        /// 2) A type with 1 explicit constructor, with at least 1 parameter.
        /// 3) A type with more than 1 explicit constructor, but with a constructor annotated with [ReaderConstructor] and satisfies 2).
        /// </summary>
        /// <param name="type">The validating class.</param>
        /// <param name="constructor">The construction method with the constructor symbol.</param>
        /// <returns>Whether the class constructor is valid or not.</returns>
        private bool ValidateConstructor(ISymbol type, [NotNullWhen(true)] out (IMethodSymbol? Symbol, ObjectConstructionStrategy Strategy)? constructor)
        {
            if (type is not INamedTypeSymbol namedType)
            {
                constructor = null;
                return false;
            }
            
            IMethodSymbol? ctorWithAttribute = null;
            IMethodSymbol? lonePublicCtor = null;

            var publicConstructors = namedType.GetExplicitlyDeclaredInstanceConstructors().Where(ctor => ctor.DeclaredAccessibility is Accessibility.Public).ToArray();

            if (publicConstructors.Length == 1)
            {
                lonePublicCtor = publicConstructors[0];
            }
            else
            {
                foreach (var ctor in publicConstructors)
                {
                    if (!ctor.ContainsAttribute(knownSymbols.ReaderConstructorAttributeType))
                    {
                        continue;
                    }

                    if (ctorWithAttribute is not null)
                    {
                        constructor = null;
                        ReportDiagnostic(DiagnosticDescriptors.MultipleCtorAttribute, type);
                        return false;
                    }

                    ctorWithAttribute = ctor;
                }

                if (ctorWithAttribute is null)
                {
                    ReportDiagnostic(DiagnosticDescriptors.MultipleCtorWithoutAttribute, type);
                    constructor = null;
                    return false;
                }
            }

            var constructorSymbol = ctorWithAttribute ?? lonePublicCtor;

            constructor = (constructorSymbol, constructorSymbol is null || constructorSymbol.Parameters.Length == 0 ? ObjectConstructionStrategy.ParameterlessConstructor : ObjectConstructionStrategy.ParameterizedConstructor);
            return true;
        }
        
        private ParameterGenerationSpec[] ParseConstructorParameters(in TypeToGenerate typeToGenerate, (IMethodSymbol? Symbol, ObjectConstructionStrategy Strategy) constructor)
        {
            var type = typeToGenerate.Type;
            Debug.Assert(!type.IsAbstract);

            ParameterGenerationSpec[] constructorParameters;
            
            if (constructor.Strategy == ObjectConstructionStrategy.ParameterlessConstructor)
            {
                constructorParameters = Array.Empty<ParameterGenerationSpec>();
            }
            else
            {
                Debug.Assert(constructor.Symbol != null);
                
                var symbol = constructor.Symbol;
                int paramCount = symbol.Parameters.Length;
                
                constructorParameters = new ParameterGenerationSpec[paramCount];

                for (int i = 0; i < paramCount; i++)
                {
                    var parameterInfo = symbol.Parameters[i];
                    var parameterTypeRef = EnqueueType(parameterInfo.Type, IsDiscardReaderType(parameterInfo.Type));

                    ProcessMemberCustomAttributes(parameterInfo, out bool? hasHeader);

                    constructorParameters[i] = new ParameterGenerationSpec
                    {
                        ParameterType = parameterTypeRef,
                        Name = parameterInfo.Name,
                        HasHeader = hasHeader
                    };
                }
            }

            return constructorParameters;
        }
    }
}
