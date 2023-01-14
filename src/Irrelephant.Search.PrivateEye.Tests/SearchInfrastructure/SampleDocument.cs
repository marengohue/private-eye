using Azure.Search.Documents.Indexes;
using Irrelephant.Search.PrivateEye.Core;

namespace Irrelephant.Search.PrivateEye.Tests.SearchInfrastructure;

[PrivateEye]
public class SampleDocument
{
    [SimpleField(IsFilterable = true, IsKey = true)]
    public string Id { get; init; } = string.Empty;

    [SimpleField(IsFilterable = true)]
    public int SomeNumber { get; init; } = 42;

    [SimpleField(IsFilterable = true)]
    public int SomeOtherNumber { get; init; } = 45;

    [SearchableField(IsFilterable = true)]
    public string SomeText { get; init; } = string.Empty;

    [SimpleField(IsFilterable = true)]
    public float SomeFloat { get; init; } = 42f;

    [SimpleField(IsFilterable = true)]
    public bool SomeBoolean { get; init; } = false;
}
