using DependenSee.Core.ResultTransformers;
using PowerArgs;
using System;
using System.IO;

namespace DependenSee.Commands
{
    public class JsonArgs
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
        [ArgDescription("If set, the output will be written to StdOut. All logs will be redirected to StdErr instead of StdOut.")]
        [ArgShortcut("SO")]
        [ArgDefaultValue(false)]
        public bool StdOut { get; set; }

        
        [ArgRequired(PromptIfMissing =true, IfNot = $"{nameof(StdOut)}")]
        [ArgDescription($"Path to write the result. If {nameof(StdOut)} is set, this is ignored.")]
        [ArgPosition(2)]
        [ArgShortcut("O")]
        public string OutputPath { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
    }
    internal static class JsonAction
    {
        public static void Execute(PowerArgsProgram globalArgs, JsonArgs args)
        {
            var result = globalArgs.RunDiscovery(args.StdOut);
         
            var output = JsonTransformer.Transform(result);

            if (args.StdOut)
            {
                Console.WriteLine(output);
            }
            else
            {
                File.WriteAllText(args.OutputPath, output);
            }
        }
    }
}
