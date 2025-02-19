using System.Text.Json.Serialization;

namespace SourceGeneratedJsonSerializer;

[JsonSerializable]
public partial class Stuff
{
    public int? MyInt { get; set; }

    [JsonIgnoreField]
    public string? MyString { get; set; }

    public Stuff? MyStuff { get; set; }
}

public class Program
{
    public static void Main(string[] args)
    {
        var data = new Stuff
        {
            MyInt = 12,
            MyStuff = null,
            MyString = "Should be ignored"
        };

        Console.WriteLine(data.ToJson());
    }
}