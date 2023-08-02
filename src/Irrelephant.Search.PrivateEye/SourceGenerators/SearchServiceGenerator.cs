using System.Text;
using Microsoft.CodeAnalysis;

namespace Irrelephant.Search.PrivateEye.SourceGenerators;

[Generator]
public class SearchServiceGenerator : PrivateEyeGeneratorBase
{
    protected override void GenerateComponent(StringBuilder builder, ITypeSymbol indexDocumentType)
    {
        builder.AppendLine("// <auto-generated>");
        builder.AppendLine("using System;");
        builder.AppendLine("using Azure.Search.Documents;");
        builder.AppendLine("using Irrelephant.Search.PrivateEye.Core;");
        builder.AppendLine("using Irrelephant.Search.PrivateEye.Core.Translation;");
        builder.AppendLine("using Irrelephant.Search.PrivateEye.Core.Query;");
        builder.AppendLine("using Irrelephant.Search.PrivateEye.Core.Search;");
        builder.AppendLine("using Irrelephant.Search.PrivateEye.Core.Filter;");
        builder.AppendLine("using Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;");
        builder.AppendLine();

        if (GetComponentNamespace(indexDocumentType) is { } ns)
            builder.AppendLine($"namespace {ns};");

        AppendClassHeader(builder, indexDocumentType);
        builder.AppendLine("{");
        GenerateFields(builder, indexDocumentType);
        GenerateCtor(builder, indexDocumentType);
        GenerateMethods(builder, indexDocumentType);
        builder.AppendLine("}");
    }

    private void GenerateFields(StringBuilder builder, ITypeSymbol indexDocumentType)
    {
        builder.AppendLine();
        builder.AppendLine("    private readonly ISearchQueryTranslator _searchTranslator;");
        builder.Append("    private readonly ISearchQueryExecutor<");
        builder.Append(indexDocumentType.Name);
        builder.AppendLine("> _searchQueryExecutor;");

        builder.AppendLine();
    }

    private void GenerateCtor(StringBuilder builder, ITypeSymbol indexDocumentType)
    {
        builder.Append("    public ");
        builder.Append(GetGeneratedTypeName(indexDocumentType));
        builder.Append("(ISearchQueryTranslator searchTranslator, ISearchQueryExecutor<");
        builder.Append(indexDocumentType.Name);
        builder.AppendLine("> searchQueryExecutor)");
        builder.AppendLine("    {");
        builder.AppendLine("        _searchTranslator = searchTranslator;");
        builder.AppendLine("        _searchQueryExecutor = searchQueryExecutor;");
        builder.AppendLine("    }");
        builder.AppendLine();
    }

    private void GenerateMethods(StringBuilder builder, ITypeSymbol indexDocumentType)
    {
        builder.Append("    public SearchQueryBuilder<");
        builder.Append(indexDocumentType.Name);
        builder.Append(", ");
        builder.Append(indexDocumentType.Name);
        builder.Append("SearchParameters, ");
        builder.Append(indexDocumentType.Name);
        builder.AppendLine("FilterParameters> Query()");
        builder.AppendLine("    {");
        builder.AppendLine("        return new(_searchTranslator, _searchQueryExecutor);");
        builder.AppendLine("    }");
    }

    private static void AppendClassHeader(StringBuilder builder, ITypeSymbol indexDocumentType)
    {
        builder.Append("class ");
        builder.Append(GetGeneratedTypeName(indexDocumentType));
        builder.Append(" : ISearchService<");
        builder.Append(indexDocumentType.Name);
        builder.Append(", ");
        builder.Append(indexDocumentType.Name);
        builder.Append("SearchParameters, ");
        builder.Append(indexDocumentType.Name);
        builder.AppendLine("FilterParameters>");
    }

    protected override string GetComponentName() => "SearchService";

    private static string GetGeneratedTypeName(ITypeSymbol indexDocumentType) =>
        indexDocumentType.Name + "SearchService";
}
