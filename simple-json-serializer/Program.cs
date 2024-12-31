// See https://aka.ms/new-console-template for more information

using SimpleJsonSerializer;

var stuff = new Stuff
{
    MyInt = null,
    MyString = "Hello world",
    MyStuff = new Stuff
    {
        MyInt = 1,
        MyString = null,
        MyStuff = null,
    }
};

Console.WriteLine(JsonSerializer.Serialize(stuff, new JsonSerializerSettings(UseCamelCase: true)));

class Stuff
{
    public int? MyInt { get; set; }

    public string MyString { get; set; }

    public Stuff MyStuff { get; set; }
}
