namespace Irrelephant.Search.PrivateEye.Tests.Integration.Utility;

public static class AsyncEnumerableExtensions
{
    public static async Task<IList<TItem>> ToListAsync<TItem>(this IAsyncEnumerable<TItem> items)
    {
        var list = new List<TItem>();
        await foreach (var item in items)
        {
            list.Add(item);
        }

        return list;
    }
}
