using FluentAssertions;
using Irrelephant.Search.PrivateEye.Tests.Integration.Fixtures;
using Irrelephant.Search.PrivateEye.Tests.Integration.Utility;

namespace Irrelephant.Search.PrivateEye.Tests.Integration;

public class IntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public IntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CanQuery_Search()
    {
        var results = await _fixture.SearchService.Query().ExecuteAsync();
        var items = await results.GetResultsAsync().ToListAsync();
        items.Count.Should().Be(3);
    }

    [Fact]
    public async Task Can_FullTextSearch()
    {
        var results = await _fixture.SearchService.Query()
            .Search(it => it.FullText.Matches("bacon"))
            .ExecuteAsync();

        var items = await results.GetResultsAsync().ToListAsync();
        items.Count.Should().Be(1);
    }

    [Fact]
    public async Task Can_UseOrInQuery()
    {
        var results = await _fixture.SearchService.Query()
            .Search(it => it.SomeText.Matches("bacon") || it.FullText.Matches("lettuce"))
            .ExecuteAsync();

        var items = await results.GetResultsAsync().ToListAsync();
        items.Count.Should().Be(2);
    }

    [Fact]
    public async Task Can_UseAndNotInQuery()
    {
        var results = await _fixture.SearchService.Query()
            .Search(it => it.FullText.Matches("the") && !it.SomeText.Matches("bacon"))
            .ExecuteAsync();

        var items = await results.GetResultsAsync().ToListAsync();
        items.Count.Should().Be(1);
    }
}
