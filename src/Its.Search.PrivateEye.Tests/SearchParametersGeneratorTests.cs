using Its.Search.PrivateEye.Core.Search;
using Its.Search.PrivateEye.Tests.SearchInfrastructure;

namespace Its.Search.PrivateEye.Tests;

public class SearchParametersGeneratorTests
{
    private SampleDocumentSearchParameters _parameters;

    public SearchParametersGeneratorTests()
    {
        _parameters = new SampleDocumentSearchParameters();
    }

    [Fact]
    public void GeneratedParams_ShouldAlwaysHaveFullTextSearch()
    {
        _parameters.FullText.Should().NotBeNull();
        _parameters.FullText.Should().BeOfType<FullTextSearchParameters>();
    }

    [Fact]
    public void GeneratedParams_ShouldHaveFullTextProp_ForEverySearchableField()
    {
        _parameters.SomeText.Should().NotBeNull();
        _parameters.SomeText.Should().BeOfType<SearchField<string>>();
    }
}
