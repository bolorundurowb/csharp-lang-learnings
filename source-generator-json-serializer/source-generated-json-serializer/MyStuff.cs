
using JsonSerializerGenerators;

namespace SourceGeneratedJsonSerializer;

[SourceJsonSerializable]
public partial class MyStuff
{
    public string Name { get; set; }
    
    [SourceJsonIgnoreField]
    public string Secret { get; set; }

    public int Age;
}
