namespace simple_json_serializer;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class JsonFieldAttribute : Attribute
{
    public string? Name { get; set; }
}
