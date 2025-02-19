using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace JsonSourceGenerator;

[Generator]
public class JsonObjectSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                (node, _) => node is ClassDeclarationSyntax cds &&
                             cds.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == "JsonSerializableAttribute")),
                (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
            .Where(cds => cds != null);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classDeclarations, SourceProductionContext context)
    {
        foreach (var classDeclaration in classDeclarations)
        {
            var model = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
            if (symbol == null) continue;

            var className = symbol.Name;
            var namespaceName = symbol.ContainingNamespace.ToDisplayString();

            var properties = symbol.GetMembers().OfType<IPropertySymbol>()
                .Where(p => p.DeclaredAccessibility == Accessibility.Public &&
                            !p.GetAttributes().Any(a => a.AttributeClass?.Name == "JsonIgnoreFieldAttribute"))
                .Select(p => p.Name);

            var fields = symbol.GetMembers().OfType<IFieldSymbol>()
                .Where(f => f.DeclaredAccessibility == Accessibility.Public &&
                            !f.GetAttributes().Any(a => a.AttributeClass?.Name == "JsonIgnoreFieldAttribute"))
                .Select(f => f.Name);

            var sb = new StringBuilder();
            sb.AppendLine($"namespace {namespaceName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public partial class {className}");
            sb.AppendLine("    {");
            sb.AppendLine("        public string ToJson()");
            sb.AppendLine("        {");
            sb.AppendLine("            var json = new System.Text.StringBuilder();");
            sb.AppendLine("            json.Append(\"{\");");

            bool isFirst = true;
            foreach (var property in properties)
            {
                if (!isFirst) sb.AppendLine("            json.Append(\",\");");
                sb.AppendLine($"            json.Append($\"\\\"{property}\\\":\\\"{{this.{property}}}\\\"\");");
                isFirst = false;
            }

            foreach (var field in fields)
            {
                if (!isFirst) sb.AppendLine("            json.Append(\",\");");
                sb.AppendLine($"            json.Append($\"\\\"{field}\\\":\\\"{{this.{field}}}\\\"\");");
                isFirst = false;
            }

            sb.AppendLine("            json.Append(\"}\");");
            sb.AppendLine("            return json.ToString();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            context.AddSource($"{className}_Json.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }
}