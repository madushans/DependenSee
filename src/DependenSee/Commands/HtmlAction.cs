using DependenSee.Core.ResultTransformers;
using PowerArgs;
using System.IO;

namespace DependenSee.Commands
{

    public class HtmlArgs
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
        [ArgDescription("Document title for Html output. If not provided, the name of the folder will be used.")]
        [ArgShortcut("T")]
        // [ArgDefaultValue("DependenSee")] defaults to source folder name
        public string Title { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        [ArgDescription("Path to write the visualizer result.")]
        [ArgShortcut("O")]
        [ArgPosition(2)]
        public string OutputPath { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
    }


    internal static class HtmlAction
    {
        public static void Execute(PowerArgsProgram globalArgs, HtmlArgs args)
        {
            var result =  globalArgs.RunDiscovery(false);
            
            var defaultTitle = new DirectoryInfo(globalArgs.SourceFolder).Name;

            var output = HtmlTransformer.Transform(result, args.Title ?? defaultTitle);

            File.WriteAllText(args.OutputPath, output);
        }
    }
}
