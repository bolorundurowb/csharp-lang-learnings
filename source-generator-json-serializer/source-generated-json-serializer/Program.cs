using source_generated_json_serializer;

var data = new MyDataClass
{
    MyInt = 12,
    MyFloat = 34.56f,
    MyString = "Should be ignored"
};

Console.WriteLine(data.ToJson());