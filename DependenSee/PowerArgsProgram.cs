using PowerArgs;

namespace DependenSee
{
    public enum OutputTypes
    {
        [ArgDescription("Creates an html document.")] Html,
        [ArgDescription("Creates a JSON file.")] Json,
        [ArgDescription("Creates a XML file.")] Xml,
        [ArgDescription("Creates a Graphviz/DOT file.")] Graphviz,
        [ArgDescription("Writes JSON output to stdout")] ConsoleJson,
        [ArgDescription("Writes XML output to stdout")] ConsoleXml,
        [ArgDescription("Writes Graphviz output to stdout")] ConsoleGraphviz,
    }

    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    [ArgDescription(@"


    ==================================================================
    |                                                                |
    |   For full documentation, support and reporting bugs           |
    |   Please visit  https://www.github.com/madushans/DependenSee   |
    |                                                                |
    ==================================================================
")]
    public class PowerArgsProgram
    {
        [HelpHook]
        [ArgDescription("Shows help descriptions.")]
        public bool Help { get; set; }
        [ArgRequired]
        [ArgPosition(0)]
        [ArgDescription("Root folder (usually solution folder) to look for csproj files recursively.")]
        [ArgExistingDirectory]
        public string SourceFolder { get; set; }

        [ArgDescription("Path to write the result. Not required if writing the output to stdout")]
        [ArgPosition(1)]
        public string OutputPath { get; set; }

        [ArgDefaultValue(false)]
        [ArgDescription("Whether to include external (Nuget) packages in the result.")]
        [ArgShortcut("P")]
        public bool IncludePackages { get; set; }

        [ArgDefaultValue(OutputTypes.Html)]
        [ArgDescription("Type of the output.")]
        [ArgShortcut("T")]
        public OutputTypes OutputType { get; set; }

        [ArgDefaultValue("")]
        [ArgDescription("Comma separated list of project file prefixes to include. Wildcards not allowed. Only the filename is considered, case insensitive. Ex:'MyApp.Core, MyApp.Extensions' Includes only projects starting with MyApp.Core and projects starting with MyApp.Extensions")]
        [ArgShortcut("IPrN")]
        public string IncludeProjectNamespaces { get; set; }

        [ArgDescription("Comma separated list of project file prefixes to exclude. Wildcards not allowed. Only the filename is considered, case insensitive. This must be a subset of includes to be useful. Ex: 'MyApp.Extensions, MyApp.Helpers' Excludes projects starting with MyApp.Extensions and projects starting with MyApp.Helpers")]
        [ArgShortcut("EPrN")]
        public string ExcludeProjectNamespaces { get; set; }

        [ArgDefaultValue("")]
        [ArgDescription("Comma separated list of package name prefixes to include. Wildcards not allowed. Only the package name is considered, case insensitive. If specified, 'IncludePackages' is overridden to True. Ex: 'Xamarin, Microsoft' includes only packages starting with Xamarin and packages starting with Microsoft")]
        [ArgShortcut("IPaN")]
        public string IncludePackageNamespaces { get; set; }

        [ArgDescription("Comma separated list of package name prefixes to exclude. Wildcards not allowed. Only the filename is considered, case insensitive. If specified, 'IncludePackages' is overridden to True. This must be a subset of includes to be useful. Ex: 'Microsoft.Logging, Azure' Excludes packages starting with Microsoft.Logging and packages starting with Azure")]
        [ArgShortcut("EPaN")]
        public string ExcludePackageNamespaces { get; set; }

        public void Main()
        {
            var service = new ReferenceDiscoveryService
            {
                ExcludePackageNamespaces = ExcludePackageNamespaces,
                ExcludeProjectNamespaces = ExcludeProjectNamespaces,
                IncludePackageNamespaces = IncludePackageNamespaces,
                IncludeProjectNamespaces = IncludeProjectNamespaces,

                IncludePackages = IncludePackages,

                OutputPath = OutputPath,
                OutputType = OutputType,

                SourceFolder = SourceFolder,
            };
            var result = service.Discover();
            new ResultWriter().Write(result, OutputType, OutputPath);
        }
    }
}
