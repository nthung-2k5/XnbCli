using Microsoft.CodeAnalysis;

namespace XnbReader.Generator;

internal static class DiagnosticDescriptors
{
    private const string Category = "GenerateXnbReadable";

    public static readonly DiagnosticDescriptor MustBePartial = new(
        id: "XNBSERI001",
        title: "XnbReadable type must be partial",
        messageFormat: "The XnbReadable type '{0}' must be partial",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MustInheritXnbContentReader = new(
        id: "XNBSERI002",
        title: "XnbReadable type does not inherit XnbContentReader",
        messageFormat: "The XnbReadable type '{0}' must inherit XnbContentReader",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MultipleCtorWithoutAttribute = new(
        id: "XNBSERI003",
        title: "Require [ReaderConstructor] when exists multiple constructors",
        messageFormat: "The XnbReadable type '{0}' must annotated with [ReaderConstructor] when exists multiple constructors",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MultipleCtorAttribute = new(
        id: "XNBSERI004",
        title: "[ReaderConstructor] exists in multiple constructors",
        messageFormat: "Multiple [ReaderConstructor] exists in '{0}' but allows only single constructor",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    
    public static readonly DiagnosticDescriptor UnsupportedCollection = new(
        id: "XNBSERI005",
        title: "Unsupported collection in class",
        messageFormat: "The XnbReadable type '{0}' member '{1}' type is '{2}' that is an unsupported collection (by MonoGame). Only Array, List, Dictionary and MemoryOwner are supported.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InterfaceCustomReaderInUnmanagedType = new(
        id: "XNBSERI008",
        title: "Unmanaged struct shouldn't inherit ICustomReader",
        messageFormat: "The unmanaged struct '{0}' shouldn't inherit with ICustomReader because unmanaged struct is deserialized as-is in memory",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor UnmanagedStructConstructor = new(
        id: "XNBSERI009",
        title: "Unmanaged struct shouldn't annotate [ReaderConstructor]",
        messageFormat: "The unmanaged struct '{0}' shouldn't annotate with [ReaderConstructor] because unmanaged struct is deserialized as-is in memory",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
