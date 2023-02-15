using System;
using DependenSee.Core;

namespace DependenSee;

public class ConsoleLogger : IDiscoveryLogger
{
    private readonly bool useStdErr;

    public ConsoleLogger(bool useStdErr)
    {
        this.useStdErr = useStdErr;
    }
    public void LogError(string message)
    {
        var revertTo = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        if (useStdErr)
        {
            Console.Error.WriteLine(message);
            Console.Error.WriteLine();
        }
        else
        {
            Console.WriteLine(message);
            Console.WriteLine();
        }
        Console.ForegroundColor = revertTo;
    }

    public void LogInfo(string message)
    {
        if (useStdErr)
        {
            Console.Error.WriteLine(message);
            Console.Error.WriteLine();
        }
        else
        {
            Console.WriteLine(message);
            Console.WriteLine();
        }
    }

    public void LogWarn(string message)
    {
        var revertTo = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        if (useStdErr)
        {
            Console.Error.WriteLine(message);
            Console.Error.WriteLine();
        }
        else
        {
            Console.WriteLine(message);
            Console.WriteLine();
        }
        Console.ForegroundColor = revertTo;
    }

    
}
