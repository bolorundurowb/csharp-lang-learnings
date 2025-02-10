using json_source_generator;

namespace source_generated_json_serializer;

[JsonSerializable]
public class MyDataClass
{
    public int? MyInt { get; set; }

    public float? MyFloat { get; set; }

    [JsonIgnoreField] public string? MyString { get; set; }
}