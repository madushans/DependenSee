namespace DependenSee;

internal class ResultWriter
{
    private const string HtmlTemplateToken = "'{#SOURCE_TOKEN#}'";
    private const string HtmlTitleToken = "{#TITLE_TOKEN#}";
    private const string HtmlTemplateName = "HtmlResultTemplate.html";
    private JsonSerializerOptions _JsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true };

    internal void Write(DiscoveryResult result, OutputTypes type, string outputPath, string htmlTitle)
    {
        switch (type)
        {
            case OutputTypes.Html:
            case OutputTypes.Xml:
            case OutputTypes.Json:
            case OutputTypes.Graphviz:
            case OutputTypes.Mermaid:
                if (string.IsNullOrWhiteSpace(outputPath))
                {
                    Console.Error.WriteLine($"output type {type} require specifying {nameof(outputPath)}");
                    return;
                }
                break;
        }

        switch (type)
        {
            case OutputTypes.Html:
                WriteAsHtmlToFile(result, outputPath, htmlTitle);
                break;
            case OutputTypes.Xml:
                WriteAsXmlToFile(result, outputPath);
                break;
            case OutputTypes.Json:
                WriteAsJsonToFile(result, outputPath);
                break;
            case OutputTypes.Graphviz:
                WriteAsGraphvizToFile(result, outputPath);
                break;
            case OutputTypes.Mermaid:
                WriteAsMermaidToFile(result, outputPath);
                break;
            case OutputTypes.ConsoleJson:
                WriteAsJsonToConsole(result);
                break;
            case OutputTypes.ConsoleXml:
                WriteAsXmlToConsole(result);
                break;
            case OutputTypes.ConsoleGraphviz:
                WriteAsGraphvizToConsole(result);
                break;
            case OutputTypes.ConsoleMermaid:
                WriteAsMermaidToConsole(result);
                break;
            default:
                throw new Exception($"Unknown {nameof(type)} '{type}'");
        }
    }

    private void WriteAsHtmlToFile(DiscoveryResult result, string outputPath, string title)
    {
        var templatePath = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, HtmlTemplateName);
        var template = File.ReadAllText(templatePath);
        var html = template
            .Replace(HtmlTemplateToken, JsonSerializer.Serialize(result, _JsonSerializerOptions))
            .Replace(HtmlTitleToken, WebUtility.HtmlEncode(title));

        File.WriteAllText(outputPath, html);
        Console.WriteLine($"HTML output written to {outputPath}");
    }

    private void WriteAsXmlToConsole(DiscoveryResult result)
    {
        var serializer = new XmlSerializer(typeof(DiscoveryResult));
        using var writeStream = Console.Out;
        serializer.Serialize(writeStream, result);
    }

    private void WriteAsXmlToFile(DiscoveryResult result, string outputPath)
    {
        var serializer = new XmlSerializer(typeof(DiscoveryResult));
        using var writeStream = File.OpenWrite(outputPath);
        serializer.Serialize(writeStream, result);
        Console.WriteLine($"XML output written to {outputPath}");
    }

    private void WriteAsJsonToFile(DiscoveryResult result, string outputPath)
    {
        File.WriteAllText(outputPath, JsonSerializer.Serialize(result, _JsonSerializerOptions));
        Console.WriteLine($"JSON output written to {outputPath}");
    }
    private void WriteAsJsonToConsole(DiscoveryResult result) =>
        Console.WriteLine(JsonSerializer.Serialize(result, _JsonSerializerOptions));

    private static void WriteAsGraphvizToConsole(DiscoveryResult result)
        => Console.WriteLine(GraphvizSerializer.ToString(result));

    private static void WriteAsGraphvizToFile(DiscoveryResult result, string outputPath)
    {
        File.WriteAllText(outputPath, GraphvizSerializer.ToString(result));
        Console.WriteLine($"GraphViz output written to {outputPath}");
    }

    private static void WriteAsMermaidToConsole(DiscoveryResult result)
        => Console.WriteLine(MermaidSerializer.ToString(result));

    private static void WriteAsMermaidToFile(DiscoveryResult result, string outputPath)
    {
        File.WriteAllText(outputPath, MermaidSerializer.ToString(result));
        Console.WriteLine($"Mermaid output written to {outputPath}");
    }
}
