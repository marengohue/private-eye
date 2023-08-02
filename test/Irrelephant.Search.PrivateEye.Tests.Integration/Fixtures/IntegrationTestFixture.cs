using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Irrelephant.Search.PrivateEye.Core;
using Irrelephant.Search.PrivateEye.Core.Query;
using Irrelephant.Search.PrivateEye.Core.Translation;
using Irrelephant.Search.PrivateEye.Tests.Integration.SearchInfrastructure;
using Microsoft.Extensions.Configuration;

namespace Irrelephant.Search.PrivateEye.Tests.Integration.Fixtures;

public class IntegrationTestFixture : IAsyncLifetime
{
    public SearchClient SearchClient { get; private set; } = null!;

    public ISearchService<SampleDocument, SampleDocumentSearchParameters, SampleDocumentFilterParameters>
        SearchService { get; private set; } = null!;

    private string _runId = Guid.NewGuid().ToString("D").Split("-").First();

    private SearchConfig _searchConfig = null!;

    private SearchIndexClient _searchIndexClient = null!;

    public async Task InitializeAsync()
    {
        _searchConfig = SetupConfiguration();
        await SetupSearchComponents();
    }

    private SearchConfig SetupConfiguration() =>
        new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.local.json", optional: true)
            .Build()
            .GetSection("AzureSearch")
            .Get<SearchConfig>()!;

    private async Task SetupSearchComponents()
    {
        _searchIndexClient = new SearchIndexClient(
            _searchConfig.Endpoint,
            new AzureKeyCredential(_searchConfig.Key)
        );
        await DeployTestIndex();
        SearchClient = new SearchClient(
            _searchConfig.Endpoint,
            GetIndexName(),
            new AzureKeyCredential(_searchConfig.Key)
        );
        SearchService = new SampleDocumentSearchService(
            new LuceneQueryTranslator(),
            new DefaultSearchQueryExecutor<SampleDocument>(SearchClient)
        );
        await SeedTestData();
        await AwaitIndexInitializedAsync();
    }

    private async Task SeedTestData()
    {
        await SearchClient.UploadDocumentsAsync(TestData.Documents);
    }

    private async Task DeployTestIndex()
    {
        var fields = new FieldBuilder().Build(typeof(SampleDocument)).ToArray();
        var index = new SearchIndex(GetIndexName(), fields);
        await _searchIndexClient.CreateIndexAsync(index);
    }

    public async Task DisposeAsync() =>
        await TearDownSearchIndex();

    private async Task TearDownSearchIndex() =>
        await _searchIndexClient.DeleteIndexAsync(GetIndexName());

    private async Task AwaitIndexInitializedAsync()
    {
        try
        {
            int attemptsLeft = 5;

            do
            {
                var result = await SearchClient.SearchAsync<SampleDocument>(
                    "*",
                    new SearchOptions { IncludeTotalCount = true }
                );
                if (result.Value.TotalCount == TestData.Documents.Length)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    return;
                }

                await Task.Delay(TimeSpan.FromSeconds(2));
            } while (--attemptsLeft > 0);

            throw new InvalidOperationException(
                "Index failed to reach expected state within the allocated time. Tearing down..."
            );
        }
        catch (Exception)
        {
            await TearDownSearchIndex();
            throw;
        }
    }

    private string GetIndexName() => $"{_searchConfig.IndexName}-{_runId}";
}
