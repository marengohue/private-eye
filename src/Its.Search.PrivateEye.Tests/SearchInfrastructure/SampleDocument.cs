using Azure.Search.Documents.Indexes;
using Its.Search.PrivateEye.Core;

namespace Its.Search.PrivateEye.Tests.SearchInfrastructure;

[PrivateEye]
public record SampleDocument
{
    [SimpleField(IsFilterable = true)]
    public string Id { get; init; } = string.Empty;

    [SimpleField(IsFilterable = true)]
    public int SomeNumber { get; init; } = 42;

    [SearchableField]
    public string SomeText { get; init; } = string.Empty;
}
