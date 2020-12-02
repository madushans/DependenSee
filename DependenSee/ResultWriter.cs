using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace DependenSee
{
    internal class ResultWriter
    {
        private const string HtmlTemplateToken = "'{#SOURCE_TOKEN#}'";
        private const string HtmlTemplateName = "HtmlResultTemplate.html";
        internal void Write(DiscoveryResult result, OutputTypes type, string outputPath)
        {
            switch (type)
            {
                case OutputTypes.Html:
                case OutputTypes.Xml:
                case OutputTypes.Json:
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
                    WriteAsHtmlToFile(result, outputPath);
                    break;
                case OutputTypes.Xml:
                    WriteAsXmlToFile(result, outputPath);
                    break;
                case OutputTypes.Json:
                    WriteAsJsonToFile(result, outputPath);
                    break;
                case OutputTypes.ConsoleJson:
                    WriteAsJsonToConsole(result);
                    break;
                case OutputTypes.ConsoleXml:
                    WriteAsXmlToConsole(result);
                    break;
                default:
                    throw new Exception($"Unknown {nameof(type)} '{type}'");
            }
        }

        private void WriteAsHtmlToFile(DiscoveryResult result, string outputPath)
        {
            var templatePath = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, HtmlTemplateName);
            var template = File.ReadAllText(templatePath);
            var html = template.Replace(HtmlTemplateToken, JsonConvert.SerializeObject(result, Formatting.Indented));

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
            File.WriteAllText(outputPath, JsonConvert.SerializeObject(result, Formatting.Indented));
            Console.WriteLine($"JSON output written to {outputPath}");
        }
        private void WriteAsJsonToConsole(DiscoveryResult result) =>
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
}
