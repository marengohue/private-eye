using Irrelephant.Search.PrivateEye.Core.Query;

namespace Irrelephant.Search.PrivateEye.Core;

public interface ISearchService<TIndexDocument, TSearchParams, TFilterParams>
{
    QueryBuilder<TIndexDocument, TSearchParams, TFilterParams> Query();
}
