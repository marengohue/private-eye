using Azure.Search.Documents.Indexes;
using Irrelephant.Search.PrivateEye.Core;

namespace Irrelephant.Search.PrivateEye.Tests.Integration.SearchInfrastructure;

[PrivateEye]
public class SampleDocument
{
    [SimpleField(IsFilterable = true, IsKey = true)]
    public string Id { get; init; } = string.Empty;

    [SimpleField(IsFilterable = true)]
    public int SomeNumber { get; init; } = 42;

    [SearchableField(IsFilterable = true)]
    public string SomeText { get; init; } = string.Empty;

}
