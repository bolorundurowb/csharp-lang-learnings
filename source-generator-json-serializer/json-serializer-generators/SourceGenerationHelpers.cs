namespace JsonSerializerGenerators;

public static class SourceGenerationHelpers
{
    public const string ObjectLevelAttribute = """
                                               namespace JsonSerializerGenerators;
                                               [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                                               public class SourceJsonSerializableAttribute : Attribute { }
                                               """;

    public const string FieldLevelAttribute = """
                                              namespace JsonSerializerGenerators;
                                              [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
                                              public class SourceJsonIgnoreFieldAttribute : Attribute { }
                                              """;
}