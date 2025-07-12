// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Text.RegularExpressions;

var filePath = "example.wispy";
var fileContents = File.ReadAllText(filePath);
var tokens = Lex(fileContents);

Console.WriteLine("Hello, World!");
return;

List<Token> Lex(string input)
{
    var chars = input.ToCharArray();
    var xen = ConsumeNextWord(chars).ToList();
    return ConsumeNextWord(chars)
        .Select(IdentifyToken)
        .ToList();
}

IEnumerable<string> ConsumeNextWord(char[] input)
{
    var tokenSb = new StringBuilder();

    foreach (var currentChar in input)
    {
        var isTokenEmpty = tokenSb.Length == 0;

        // if there is a whitespace character after a token then the token can be considered terminated
        if (char.IsWhiteSpace(currentChar))
        {
            if (isTokenEmpty)
                continue;

            yield return ExtractAndReset();
            continue;
        }

        if (IsTerminatorToken(currentChar))
        {
            if (!isTokenEmpty)
                yield return ExtractAndReset();

            yield return currentChar.ToString();
            continue;
        }

        tokenSb.Append(currentChar);
    }

    yield break;

    string ExtractAndReset()
    {
        var token = tokenSb.ToString();
        tokenSb.Clear();

        return token;
    }
}

Token IdentifyToken(string input)
{
    if (IsInt(input))
        return new IntToken(input);

    if (IsFloat(input))
        return new FloatToken(input);

    if (IsIdentifier(input))
        return new IdentifierToken(input);

    if (IsBracket(input))
        return new BracketToken(input);

    if (IsTypedIdentifier(input))
        return new TypedIdentifierToken(input);

    throw new Exception("Unknown token type");
}

bool IsTerminatorToken(char input) => IsBracket(input.ToString());

bool IsInt(string input) => Regex.IsMatch(input, @"^\d+$");

bool IsFloat(string input) => Regex.IsMatch(input, @"^[0-9]+\.[0-9]+$");

bool IsIdentifier(string input) => Regex.IsMatch(input, @"^[a-zA-Z_][a-zA-Z0-9_\-]*$");

bool IsBracket(string input) => Regex.IsMatch(input, @"[\(\)\[\]]");

bool IsTypedIdentifier(string input) => Regex.IsMatch(input, @"^[a-zA-Z_][a-zA-Z0-9_\-]*:[a-zA-Z_][a-zA-Z0-9_\-]*$");

class IntToken(string rawValue) : Token
{
    public override TokenType Type => TokenType.Integer;

    public int Value => int.Parse(rawValue);
}

class FloatToken(string rawValue) : Token
{
    public override TokenType Type => TokenType.FloatingPoint;

    public float Value => float.Parse(rawValue);
}

class BracketToken(string rawValue) : Token
{
    public override TokenType Type => TokenType.Bracket;

    public char Value => rawValue.Trim()[0];
}

class IdentifierToken(string rawValue) : Token
{
    public override TokenType Type => TokenType.Identifier;

    public string Value => rawValue;
}

class TypedIdentifierToken(string rawValue) : Token
{
    public override TokenType Type => TokenType.TypedIdentifier;

    public string Value => rawValue;
}

abstract class Token
{
    public abstract TokenType Type { get; }
}

enum TokenType
{
    Integer,
    FloatingPoint,
    Identifier,
    TypedIdentifier,
    Bracket
}