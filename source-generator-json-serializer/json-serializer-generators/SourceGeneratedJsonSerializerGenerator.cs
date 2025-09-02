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
                    predicate: static (s, _) => true,
                    transform: static (s, _) => (ClassDeclarationSyntax)s.TargetNode
                )
                .Combine(context.CompilationProvider);
            
            context.RegisterSourceOutput(classesToUpdate, static (spc, source) => Execute(source.Right, source.Left, spc));
        });
    }

    private static void Execute(Compilation compilation, ClassDeclarationSyntax source, SourceProductionContext spc)
    {
        var partialClass = SourceGenerationHelpers.GeneratePartialClass(source);
        spc.AddSource($"{source.Identifier.Text}.g.cs", SourceText.From(partialClass, Encoding.UTF8));
    }
}