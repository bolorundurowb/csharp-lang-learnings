using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace JsonSerializerGenerators;

[Generator]
public class SourceGeneratedJsonSerializerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("SourceJsonSerializableAttribute.g.cs", SourceGenerationHelpers.ObjectLevelAttribute);
            ctx.AddSource("SourceJsonIgnoreFieldAttribute.g.cs", SourceGenerationHelpers.FieldLevelAttribute);

            var classesToUpdate = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "JsonSerializerGenerators.SourceJsonSerializableAttribute",
                    predicate: static (node, _) => node is ClassDeclarationSyntax,
                    transform: static (syntaxCtx, _) => (INamedTypeSymbol)syntaxCtx.TargetSymbol
                )
                .Combine(context.CompilationProvider);
            
            context.RegisterSourceOutput(classesToUpdate, static (spc, source) => Execute(source.Right, source.Left, spc));
        });
    }

    private static void Execute(Compilation compilation, INamedTypeSymbol sourceSymbol, SourceProductionContext spc)
    {
        var partialClass = SourceGenerationHelpers.GeneratePartialClass(sourceSymbol, compilation);
        spc.AddSource($"{sourceSymbol.Name}.g.cs", SourceText.From(partialClass, Encoding.UTF8));
    }
}