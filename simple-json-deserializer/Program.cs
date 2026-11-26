// See https://aka.ms/new-console-template for more information

using simple_json_deserializer;

var jsonInput = "null";
var result = JsonDeserialiser.Deserialise<MyType>(jsonInput);

Console.WriteLine("Deserialised result: " + result);

class MyType
{
    public int MyProperty { get; set; }
}
