namespace simple_json_deserializer;

public class JsonDeserialiser
{
    public static T? Deserialise<T>(string input)
    {
        var cleanedInput = input.StripWhitespace();

        if (string.IsNullOrWhiteSpace(cleanedInput))
        {
            throw new ArgumentException("Input cannot be null or whitespace");
        }

        if (cleanedInput == StringUtilities.NullToken)
        {
            return default;
        }

        throw new NotImplementedException();
    }
}