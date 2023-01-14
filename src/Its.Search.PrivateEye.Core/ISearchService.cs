using Its.Search.PrivateEye.Core.Query;

namespace Its.Search.PrivateEye.Core;

public interface ISearchService<TIndexDocument, TParameters>
{
    SearchQueryBuilder<TIndexDocument, TParameters> Query();
}
