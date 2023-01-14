using System.Text;
using Microsoft.CodeAnalysis;

namespace Its.Search.PrivateEye.SourceGenerators;

[Generator]
public class SearchServiceGenerator : PrivateEyeGeneratorBase
{
    protected override void GenerateComponent(StringBuilder builder, ITypeSymbol indexDocumentType)
    {
        builder.AppendLine("// <auto-generated>");
        builder.AppendLine("using System;");
        builder.AppendLine("using Its.Search.PrivateEye.Core;");
        builder.AppendLine("using Its.Search.PrivateEye.Core.Query;");
        builder.AppendLine("using Its.Search.PrivateEye.Core.Search;");
        builder.AppendLine("using Its.Search.PrivateEye.Core.Filter;");

        if (GetComponentNamespace(indexDocumentType) is { } ns)
            builder.AppendLine($"namespace {ns};");

        AppendClassHeader(builder, indexDocumentType);
        builder.AppendLine("{");
        GenerateMethods(builder, indexDocumentType);
        builder.AppendLine("}");
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
        builder.AppendLine("        return new();");
        builder.AppendLine("    }");
    }

    private static void AppendClassHeader(StringBuilder builder, ITypeSymbol indexDocumentType)
    {
        builder.Append("class ");
        builder.Append(indexDocumentType.Name);
        builder.Append("SearchService : ISearchService<");
        builder.Append(indexDocumentType.Name);
        builder.Append(", ");
        builder.Append(indexDocumentType.Name);
        builder.Append("SearchParameters, ");
        builder.Append(indexDocumentType.Name);
        builder.AppendLine("FilterParameters>");
    }

    protected override string GetComponentName() => "SearchService";
}
