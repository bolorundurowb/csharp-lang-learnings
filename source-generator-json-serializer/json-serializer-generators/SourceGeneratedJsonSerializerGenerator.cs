using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace JsonSerializerGenerators;

public class SourceGeneratedJsonSerializerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("SourceJsonSerializableAttribute.g.cs", SourceGenerationHelpers.ObjectLevelAttribute);
            ctx.AddSource("SourceJsonIgnoreFieldAttribute.g.cs", SourceGenerationHelpers.FieldLevelAttribute);
        });
    }
}