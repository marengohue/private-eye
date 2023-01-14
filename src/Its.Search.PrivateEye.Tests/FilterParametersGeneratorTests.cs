using Its.Search.PrivateEye.Core.Filter;
using Its.Search.PrivateEye.Tests.SearchInfrastructure;

namespace Its.Search.PrivateEye.Tests;

public class FilterParametersGeneratorTests
{
    private readonly SampleDocumentFilterParameters _parameters;

    public FilterParametersGeneratorTests()
    {
        _parameters = new();
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
        _parameters.Id.Should().BeOfType<SimpleFilterField<string>>();
        _parameters.SomeNumber.Should().BeOfType<SimpleFilterField<int>>();
        _parameters.SomeText.Should().BeOfType<SearchableFilterField<string>>();
    }


}
