// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("File path and output file path must be provided as arguments.");
        }

        var inputPath = args[0];
        var outputPath = args[1];

        if (!File.Exists(inputPath))
        {
            throw new FileNotFoundException("Input file not found.", inputPath);
        }

        var rawMdContent = await File.ReadAllLinesAsync(inputPath);

        if (rawMdContent.Length == 0)
        {
            throw new ArgumentException("Input file is empty.");
        }

        var outputSb = new StringBuilder();

        for (var i = 0; i < rawMdContent.Length; i++)
        {
            var line = rawMdContent[i];
            var isSingleLine = IsSingleLine(line);

            if (isSingleLine)
            {
                ConvertToHtml(outputSb, line);
            }
            else
            {
                // combine lines 
            }
        }
    }

    private static bool IsSingleLine(string line)
    {
        var trimmedLine = line.Trim();
        // unordered lists
        return !trimmedLine.StartsWith('*')
               // tables
               && !trimmedLine.StartsWith('|')
               // block quotes
               && !trimmedLine.StartsWith('>')
               // code blocks
               && !trimmedLine.StartsWith("```")
               // ordered lists
               && !Regex.IsMatch(line, @"^(\d)+\.(\s)+");
    }

    private static void ConvertToHtml(StringBuilder sb, string line)
    {
        var headerLevel = line.TakeWhile(x => x == '#').Count();
        line = line.Substring(headerLevel);

        if (headerLevel > 0)
        {
            sb.Append($"<h{headerLevel}>");
        }

        for(var i = 0; i < line.Length; i++)
        {
            var character = line[i];
            
            if (character == '*')
            {
                // Handle bold or italic
                if (i + 1 < line.Length && line[i + 1] == '*')
                {
                    sb.Append("<strong>");
                    i++; // Skip the next character
                }
                else
                {
                    sb.Append("<em>");
                }
            }
            else if (character == '_')
            {
                // Handle italic
                sb.Append("<em>");
            }
            else if (character == '`')
            {
                // Handle inline code
                sb.Append("<code>");
            }
            else if (character == '~')
            {
                sb.Append("<del>");
            }
            else if (character == '\n')
            {
                sb.Append("<br/>");
            }
            else
            {
                sb.Append(character);
            }
        }

        if (headerLevel > 0)
        {
            sb.Append($"</h{headerLevel}>");
        }
    }

    private static string? ConvertToHtml(List<string> lines)
    {
        throw new NotImplementedException();
    }
}