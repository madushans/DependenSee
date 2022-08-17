namespace DependenSee;
public static class Program
{
    public static int Main(string[] args)
    {
        if (args is null || args.Length == 0)
        {
            // if no arguments specified, instead of showing an error,
            // pretend as help command
            Args.InvokeMain<PowerArgsProgram>(new[] { $"-{nameof(PowerArgsProgram.Help)}" });
            return 0;
        }
        try
        {
            var objResult = Args.InvokeMain<PowerArgsProgram>(args);
            var result = objResult.Value as PowerArgsProgram;
            if (result.ErrorCode == ErrorCodes.None)
            {
                return 0;
            }

            WriteErrorMessages(result);
            return (int)result.ErrorCode;
        }
        catch (Exception ex)
        {
            WriteUnexpectedException(ex);
            return 1;
        }
    }

    private static void WriteErrorMessages(PowerArgsProgram result)
    {
        var currentColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;

        var errorCodeType = result.ErrorCode.GetType();
        var errorCodeValueMember = errorCodeType.GetField(result.ErrorCode.ToString());
        var argDescription = errorCodeValueMember.GetCustomAttribute<ArgDescription>();

        Console.WriteLine(argDescription.Description);

        Console.ForegroundColor = currentColor;
    }


    private static void WriteUnexpectedException(Exception ex)
    {
        var currentColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;

        Console.Error.WriteLine($"{ex.GetType()}:{ex.Message}\r\n{ex.StackTrace}");

        var inner = ex.InnerException;

        while (inner != null)
        {
            Console.Error.WriteLine($"-----Inner Exception----");
            Console.Error.WriteLine($"{inner.GetType()}:{inner.Message}\r\n{inner.StackTrace}");
            inner = inner.InnerException;
        }

        Console.ForegroundColor = ConsoleColor.Red;

        Console.Error.WriteLine(@"

    =============================================================
    |                                                           |
    |                   Well that didn't work.                  |
    |                                                           |
    |   If this keeps happening,                                |
    |     - try updating to the latest version with             |
    |       - dotnet tool update DependenSee --global           |
    |                                                           |
    |     - check current version with                          |
    |       - dotnet tool list --global                         |
    |                                                           |
    |     - Please open an issue at                             |
    |       - https://www.github.com/madushans/DependenSee      |
    |                                                           |
    |              ♥ Thank you for your support ♥               |
    |                                                           |
    =============================================================
");
        Console.ForegroundColor = currentColor;
    }
}
