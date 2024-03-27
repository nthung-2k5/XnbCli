using System.Diagnostics;

namespace XnbReader.Generator.Model;

/// <summary>
/// Models a root generated type that serves as the actual content representation of the .xnb file.
/// </summary>
/// <remarks>
/// Type needs to be cacheable as a Roslyn incremental value so it must be
///
/// 1) immutable and
/// 2) implement structural (pointwise) equality comparison.
///
/// We can get these properties for free provided that we
///
/// a) define the type as an immutable C# record and
/// b) ensure all nested members are also immutable and implement structural equality.
///
/// When adding new members to the type, please ensure that these properties
/// are satisfied otherwise we risk breaking incremental caching in the source generator!
/// </remarks>
[DebuggerDisplay("ReaderFormat = {ReaderFormat}")]
public record RootTypeGenerationSpec
{
    /// <summary>
    /// The expected reader type string in the .xnb file.
    /// </summary>
    public required string ReaderFormat { get; init; }
    
    /// <summary>
    /// The type being generated.
    /// </summary>
    public required TypeGenerationSpec TypeGenSpec { get; init; }
}
