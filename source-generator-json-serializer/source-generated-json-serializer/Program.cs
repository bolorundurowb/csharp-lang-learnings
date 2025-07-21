using SourceGeneratedJsonSerializer;

var stuff = new MyStuff { Name = "Hello", Secret = "Don't show this", Age = 25 };
Console.WriteLine(stuff.ToJson());
