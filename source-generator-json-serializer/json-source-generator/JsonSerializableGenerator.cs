using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace json_source_generator;

[Generator]
public class JsonSerializableGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // Register a syntax receiver to collect candidate types
        context.RegisterForSyntaxNotifications(() => new JsonSerializableSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not JsonSerializableSyntaxReceiver receiver)
            return;

        // Get the compilation
        var compilation = context.Compilation;

        // Get the attribute symbols
        var jsonSerializableAttributeSymbol = compilation.GetTypeByMetadataName("JsonSerializableAttribute");
        var jsonIgnoreFieldAttributeSymbol = compilation.GetTypeByMetadataName("JsonIgnoreFieldAttribute");

        if (jsonSerializableAttributeSymbol == null || jsonIgnoreFieldAttributeSymbol == null)
            return;

        // Process each candidate class
        foreach (var classDeclaration in receiver.CandidateClasses)
        {
            var model = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var typeSymbol = model.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;

            if (typeSymbol == null || !typeSymbol.GetAttributes().Any(ad => ad.AttributeClass.Equals(jsonSerializableAttributeSymbol, SymbolEqualityComparer.Default)))
                continue;

            // Generate the ToJson method
            var source = GenerateToJsonMethod(typeSymbol, jsonIgnoreFieldAttributeSymbol);
            context.AddSource($"{typeSymbol.Name}_ToJson.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private string GenerateToJsonMethod(INamedTypeSymbol typeSymbol, ISymbol jsonIgnoreFieldAttributeSymbol)
    {
        var properties = typeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public && !p.GetAttributes().Any(ad => ad.AttributeClass.Equals(jsonIgnoreFieldAttributeSymbol, SymbolEqualityComparer.Default)));

        var fields = typeSymbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f => f.DeclaredAccessibility == Accessibility.Public && !f.GetAttributes().Any(ad => ad.AttributeClass.Equals(jsonIgnoreFieldAttributeSymbol, SymbolEqualityComparer.Default)));

        var sb = new StringBuilder();
        sb.AppendLine($"namespace {typeSymbol.ContainingNamespace.ToDisplayString()}");
        sb.AppendLine("{");
        sb.AppendLine($"    public partial class {typeSymbol.Name}");
        sb.AppendLine("    {");
        sb.AppendLine("        public string ToJson()");
        sb.AppendLine("        {");
        sb.AppendLine("            var json = new System.Text.StringBuilder();");
        sb.AppendLine("            json.Append(\"{\");");

        // Process properties
        bool isFirst = true;
        foreach (var property in properties)
        {
            if (!isFirst)
                sb.AppendLine("            json.Append(\",\");");
            sb.AppendLine($"            json.Append($\"\\\"{property.Name}\\\":\\\"{property.Name}\\\"\");");
            isFirst = false;
        }

        // Process fields
        foreach (var field in fields)
        {
            if (!isFirst)
                sb.AppendLine("            json.Append(\",\");");
            sb.AppendLine($"            json.Append($\"\\\"{field.Name}\\\":\\\"{field.Name}\\\"\");");
            isFirst = false;
        }

        sb.AppendLine("            json.Append(\"}\");");
        sb.AppendLine("            return json.ToString();");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}