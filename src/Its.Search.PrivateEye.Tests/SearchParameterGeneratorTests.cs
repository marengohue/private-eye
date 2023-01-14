using Its.Search.PrivateEye.Core.ParameterDefinitions;
using Its.Search.PrivateEye.Tests.SearchInfrastructure;

namespace Its.Search.PrivateEye.Tests;

public class SearchParameterGeneratorTests
{
    private readonly SampleDocumentSearchParameters _parameters;

    public SearchParameterGeneratorTests()
    {
        _parameters = new SampleDocumentSearchParameters();
    }

    [Fact]
    public void GenerateParametersObject_WhenMarked()
    {
        _parameters.Should().NotBeNull();
    }

    [Fact]
    public void GenerateParametersObject_ContainsProperties_PerMarkedPropertyInModel()
    {
        _parameters.Id.Should().NotBeNull();
        _parameters.SomeNumber.Should().NotBeNull();
        _parameters.SomeText.Should().NotBeNull();
    }

    [Fact]
    public void GeneratedParamProperties_ShouldMatchFieldType()
    {
        _parameters.Id.Should().BeOfType<SimpleFieldParameters<string>>();
        _parameters.SomeNumber.Should().BeOfType<SimpleFieldParameters<int>>();
        _parameters.SomeText.Should().BeOfType<SearchableFieldParameters>();
    }

    [Fact]
    public void GeneratedParams_ShouldHaveFullTextSearch_WhenHavingAtLeastSingleSearchableField()
    {
        _parameters.FullText.Should().NotBeNull();
        _parameters.FullText.Should().BeOfType<FullTextSearchParameters>();
    }

}
