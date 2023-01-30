using DependenSee.Commands;
using DependenSee.Core;
using PowerArgs;
using System;
using System.IO;

namespace DependenSee;

[ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
[ArgDescription(@"

    ==================================================================
    |   For full documentation, support and reporting bugs           |
    |   Please visit https://www.github.com/madushans/DependenSee    |
    ==================================================================
")]
public class PowerArgsProgram
{
    [HelpHook]
    [ArgShortcut("H")]
    [ArgDescription("Shows help descriptions.")]
    public bool Help { get; set; }

    [ArgRequired]
    [ArgPosition(1)]
    [ArgShortcut("S")]
    [ArgDescription("Root folder (usually root or src of your repository) to look for project and solution files recursively.")]
    [ArgExistingDirectory]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
    public string SourceFolder { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
   
    [ArgDefaultValue("")]
    [ArgDescription("List of folders (either absolute paths or relative to SourceFolder) to skip during scan, even if there are references to them from your projects. Separate multiple paths with the platform's path separator, usually ';'")]
    [ArgShortcut("EFol")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
    public string ExcludeFolders { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

    [ArgDefaultValue(false)]
    [ArgDescription("Set if you want the discovery to follow valid reparse points. This is helpful if your project references are relying on symlinks, NTFS junction points .etc.")]
    [ArgShortcut("FReP")]
    public bool FollowReparsePoints { get; set; }

    [ArgActionMethod]
    [ArgDescription("Creates a standalone visualizer (html) file which can be opened in a web browser.")]
    [ArgExample(
        example:"dependensee html ./repos/my-project ./my-output.html",
        description:"typical usage", 
        Title = "Positional Arguments"
        )]
    [ArgExample(
        example: "dependensee html -S ./repos/my-project -O ./my-output.html",
        description: "shortcuts for arguments",
        Title = "Named Short Arguments"
        )]
    [ArgExample(
        example: $"dependensee html -{nameof(SourceFolder)} ./repos/my-project -{nameof(HtmlArgs.OutputPath)} ./my-output.html -{nameof(HtmlArgs.Title)} My-Dependencies",
        description: "full argument names",
        Title = "Named Long Arguments"
        )]
    public void Html(HtmlArgs args)
    {
        HtmlAction.Execute(this, args);
    }

    [ArgActionMethod]
    [ArgDescription("Creates a JSON result")]
    [ArgExample(
        example: "dependensee json ./repos/my-project ./my-output.json",
        description: "typical usage",
        Title = "Positional Arguments"
        )]
    [ArgExample(
        example: "dependensee json -S ./repos/my-project -SO",
        description: "write to STDOUT",
        Title = "Named Short Arguments"
        )]
    [ArgExample(
        example: $"dependensee json -{nameof(SourceFolder)} ./repos/my-project -{nameof(JsonArgs.OutputPath)} ./my-output.html -{nameof(JsonArgs.StdOut)}",
        description: "full argument names",
        Title = "Named Long Arguments"
        )]
    public void Json(JsonArgs args)
    {
        JsonAction.Execute(this, args);
    }

    internal DiscoveryResult RunDiscovery(bool logsToErr)
    {
        var service = new DiscoveryService(logger: new ConsoleLogger(logsToErr));

        var request = new DiscoveryRequest
        {
            SourceFolder = SourceFolder,
            ExcludeFolders = string.IsNullOrWhiteSpace(ExcludeFolders)
                                ? Array.Empty<string>()
                                : ExcludeFolders.Split(Path.PathSeparator),

            FollowReparsePoints = FollowReparsePoints
        };
        
        return service.Discover(request);
    }
}

