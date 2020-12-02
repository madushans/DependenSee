using PowerArgs;

namespace DependenSee
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args is null || args.Length == 0)
            {
                // if no arguments specified, instead of showing an error,
                // pretend as help command
                Args.InvokeMain<PowerArgsProgram>(new[] { $"-{nameof(PowerArgsProgram.Help)}" });
                return;
            }

            Args.InvokeMain<PowerArgsProgram>(args);
        }
    }
}
