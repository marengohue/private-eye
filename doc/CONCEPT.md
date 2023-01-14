# Source Generators
Private Eye generates additional infrastructure for search index documents marked with ```[PrivateEye]``` attributes. Following infrastructure is generated:
- For every index document marked with ```[PrivateEye]``` attribute, system generates special Filter and Search documents as well as a search service, meant to search and filter on those documents.
- Inside the marked index documents, for every ```[SimpleField]``` and ```[SearchableField]``` marked as ```IsFilterable=true```, system generates a property in the ```FilterParameters``` type, meant for use inside odata ```$filter=``` clauses.
- For every ```[SearchableField]``` system generates a property in the ```SearchParameters``` type meant for use inside full-text search queries.
- All of the search and filter properties are strongly typed, meaning that normal C# type system applies when constructing queries.
- Generated search services allow for strongly-typed querying and filtering on the azure search data source.

# Using the search services
Generally, querying the azure search data sources is done in several steps.
1) Setting up the full-text search query.
1) Setting up the Filter query to further filter on the result source.
1) Setting up the Facets to get an enumeration of possible values of various facetable properties in the index.
1) Setting up ordering of the result set. Ordering could be performed on either search result score and or various properties set as ```IsSortable=true``` in the index.

Every individual step of this process could be omitted, in the most basic case resulting in all items in the index being enumerated in arbitrary order.

Given following index data model:
````csharp
[PrivateEye]
public class Person
{
    [SimpleField(IsKey=true)]
    public string Id { get; init; }

    [SearchableField]
    public string FirstName { get; init; }

    [SimpleField(IsFilterable=true, IsFacetable=true)]
    public string Nationality { get; init; }

    [SimpleField(IsSearchable=true, IsSortable=true)]
    public int Age { get; init; }

    [SimpleField(IsFacetable=true)]
    public string Occupation { get; init; }

    [SearchableField(IsFilterable=true)]
    public string Bio { get; init; }
}

````

In order to use the search services, one has to call it's ```Query()``` method and then use the resulting query builder to assemble the query:
````csharp
_searchService.Query()
    // Search section of the query. Query can either be connected
    // to an individual field or the whole document.
    .Search(idx => idx.Matches("magic*"))
    .Search(idx => idx.Bio.Matches("coffee*"))
    .Search(idx => idx.FirstName.Matches("Chris*"))

    // Filter section of the query
    .Where(it => it.Age >= 18)
    .Where(it => it.Nationality == "NO" || it.Nationality == "DK")
    .Where(it => !it.Bio.Matches("tea*"))

    // Facet section of the query
    .Facet(it => it.Occupation)
    .Facet(it => it.Nationality)

    // Ordering section of the query
    .OrderBy(it => it.Age)
    .ThenBy(it => it.SearchScore)

    .ToArrayAsync();
````

Which ultimately, will get converted into the following Azure Search query(TODO: confirm with docs):
````json
{
    "search": "magic* AND Bio:coffee* AND FirstName:Chris*",
    "filter": "Age gt 18 and (Nationality eq 'NO' or Nationality eq 'DK') and not search.in(Bio, 'tea*')",
    "sort": "age asc, score asc",
    "facets": ['Occupation', 'Nationality']
}
````
