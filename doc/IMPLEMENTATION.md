# Implementation Ideas
The process of taking in the data model and generating all of the necessary bits of infrastructure
could be separated into several distinct steps:

1) [Source Generation](#source-generation) Generate all components from annotated models.
   1) [Generating the parameters](#generating-the-parameters) Analyze the models containing the ```[PrivateEye]``` attribute and generate types for Search, Filter, Facet and Ordering
models.
   2) [Generating the services](#generating-the-services) Generate the search service with minimal logic to allow the user to initiate the query builder.
2) [Expression analysis](#expression-analysis) Query builder leverages familiar LINQ-like syntax to produce an expression tree for various sections of the query,
depending on the methods used.
3) [AST translation](#translating-asts) expression trees produced are then walked and text representation of the lucene query is produced before sending it into azure search.

## Source generation
### Generating the parameters
There is a distinct source generator for every parameters type that has to be produced.
The generators extract properties marked with either ```[SearchableField]``` or ```[SimpleField]``` attributes.
Depending on the values of ```IsFilterable```, ```IsFacetable``` and ```IsSortable``` properties, these properties are then
populated into various parameters objects:

- Every field marked with ```IsFilterable=true``` property is added to filter parameters object.
   - Simple fields get default filtering options based on their type (equality and/or ordinal comparisons)
   - Searchable fields get the ability to perform ```search.in()``` filtering
- Every ```[SearchableField]``` is added into the search parameters object. This object also allows to perform global
full-text search on the entire document through use of ```FullText``` property.
- Every field marked with ```IsFacetable=true``` property is added to facet parameters object.
- Evert field marked with ```IsSortable=true``` property is added to sorting parameters object. This object also allows
to perform sorting based on result search score.

### Generating the services
For every model marked with ```[PrivateEye]``` attribute, a generic search service is then generated based on the parameter types.
This service allows to initiate the query building by calling ```.Query()``` method of the service.

At this point, source generation is largely done and query construction could be performed using the Expression tree analysis.

## Expression analysis
Next step of the generation is to produce a set of expression trees for every section of the query. Sorting and Faceting is fairly simple,
so no complex expression analysis needs to be performed, but Search and Filter parts of the query have to be constructed by analysing the contents
of ```Expr<...>``` produced by the LINQ-like queries.

### Search expressions
Search expression tree is constructed by using the results of ```.Search()``` calls of the query builder.
All of the calls are concatenated using ```AND``` clauses since this is the result that chaining LINQ in normal C# would produce.
As opposed to filtering, search queries could modify the scoring parameters of the results and also could have modifiers
based on importance of certain matches, fuzzy search and so on. First implementation of this expression analyser is expected
to not takes those variables into account and produce an AST where individual sub-expressions are all equally weighted.

Example:
````csharp
_service.Query()
    .Search(it => it.Matches("fizz*") || it.Matches("buzz*"))
    .Search(it => it.Field.Matches("foo*"))
    .Search(it => it.AnotherField.Matches("bar*"));
````
Would produce the following search AST:
```
And(
    Or(
        Match(Document, Value("fizz*")),
        Match(Document, Value("buzz*"))
    )
    And(
        Match(Field("Field"), Value("foo*")),
        Match(Field("AnotherField"), Value("bar*"))
    )
)
```

It should be possible to embed external parameters into the query so that search could be made more interactive:
````csharp
var userInput = "this is a search query";
_service.Query()
    .Search(it => it.Match(userInput));
````
produces the following:
````
Match(Document, Value("this is a search query"))
````
Although, additional processing of the user input could be performed to improve the matches.

### Filter expressions
Filter expressions are analysed separately from the search expressions in order to produce a similar AST,
which is going to be translated into a different language, since search and filter expressions are not based on the same
Lucene syntax. Same rules apply to these expressions with several additions:

It should be possible to make referential filters where filtering is performed by comparing two properties of the document
at the same time:
````csharp
_service.Query()
    .Where(it => it.SomeField == it.AnotherField)
    .Where(it => it.SomeDate > it.AnotherDate)
````
would produce the following AST:
````
And(
    Eq(
        Field("SomeField"),
        Field("AnotherField")
    ),
    Eq(
        Field("SomeDate"),
        Field("AnotherDate")
    )
)
````

## Translating ASTs
Last step of the query construction is to convert the ASTs produces on the last step into their respective query languages.
Two query languages are required to query azure search:

- Lucene-like search queries
- Odata-like filter queries

Both sorting and faceting doesn't require complex tree analysis so there is no need to perform AST construction and translation for
those cases.
