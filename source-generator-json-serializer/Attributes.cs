namespace SourceGeneratedJsonSerializer;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class JsonSerializableAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class JsonIgnoreFieldAttribute : Attribute { }
