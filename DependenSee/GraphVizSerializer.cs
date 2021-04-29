using System.CodeDom.Compiler;
using System.IO;

namespace DependenSee
{
    public static class GraphvizSerializer
    {
        const string ProjectBackgroundColor = "#00cc22";
        const string PackageBackgroundColor = "#22aaee";

        public static string ToString(DiscoveryResult discoveryResult)
        {
            using var stringWriter = new StringWriter();
            using var writer = new IndentedTextWriter(stringWriter);

            writer.WriteLine("digraph dependencies {");

            writer.Indent++;

            foreach (var project in discoveryResult.Projects)
            {
                writer.WriteLine($"\"{project.Id}\" [label=\"{project.Name}\", fillcolor=\"{ProjectBackgroundColor}\", style=filled];");
            }

            foreach (var package in discoveryResult.Packages)
            {
                writer.WriteLine($"\"{package.Id}\" [label=\"{package.Name}\", fillcolor=\"{PackageBackgroundColor}\", style=filled];");
            }

            foreach (var reference in discoveryResult.References)
            {
                writer.WriteLine($"\"{reference.From}\" -> \"{reference.To}\";");
            }

            writer.Indent--;

            writer.WriteLine("}");

            writer.Flush();

            return stringWriter.ToString();
        }
    }
}
