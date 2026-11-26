namespace simple_json_deserializer;

public static class StringUtilities
{
    public const string NullToken = "null";

    public static string StripWhitespace(this string input)
    {
        var chars = input.ToCharArray();
        var filteredChars = chars.Where(x => !char.IsWhiteSpace(x)).ToArray();
        return new string(filteredChars);
    }

}