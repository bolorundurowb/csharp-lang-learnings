using SourceGeneratedJsonSerializer;

var stuff = new MyStuff();
var json = stuff.ToJson();
Console.WriteLine(json);
