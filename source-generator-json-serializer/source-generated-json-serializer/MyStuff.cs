using JsonSerializerGenerators;

namespace SourceGeneratedJsonSerializer;

[SourceJsonSerializable]
public partial class MyStuff
{
    public string Name { get; set; } = "John Doe";

    [SourceJsonIgnoreField] 
    public string Secret { get; set; } = "Don't show this";

    public int Age = 13;

    public bool IsCool { get; set; } = true;
}