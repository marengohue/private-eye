using Its.Search.PrivateEye.Core.Query;

namespace Its.Search.PrivateEye.Core;

public interface ISearchService<TIndexDocument, TSearchParams, TFilterParams>
{
    SearchQueryBuilder<TIndexDocument, TSearchParams, TFilterParams> Query();
}
