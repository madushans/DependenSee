# DependenSee
Dotnet project and package dependency visualizer.

# Install

 ***Requires .NET 5 or higher***

`dotnet tool install DependenSee --global`

available via [Nuget](https://www.nuget.org/packages/DependenSee)

# Uninstall
`dotnet tool uninstall DependenSee --global`

# How to Run

`DependenSee root/of/projects path/to/output.html`


## Sample Output (HTML)

![path/to/](https://raw.githubusercontent.com/madushans/DependenSee/main/sample-output.png)


# Why `DependenSee` over 'X'

Current popular options are to either use NDepend, VS Architecture Explorer or Code Map. While these options are feature rich, they also has a licensing cost. If all you want is to see the dependency graph, and nothing else, it may be hard to justify the licensing cost. Then `DependenSee` is for you.

If you need to see the Type structure, relationships between your methods or types .etc. then you should use one of the options above instead. DependenSee is mean to be very simple, easy, straight forward to use and FREE! `DependenSee` does not intend to compete with the above. See [Limitations](#Limitations)

# Docs

## Features

- Creates the dependency graph for your solution.
- Can only include or exclude certain namespaces so the result is not overwhelming or filled with noise.
- Can create HTML, XML, JSON outputs
- Can return XML or JSON to `STDOUT` for further processing by other command line tools
 
For full docs run without any arguments
`DependenSee`

````
Usage - DependenSee <SourceFolder> [<OutputPath>] -options

GlobalOption                       Description
Help (-H)                          Shows help descriptions.
SourceFolder* (-S)                 Root folder (usually solution folder) to look for csproj files recursively.
OutputPath (-O)                    Path to write the result. Not required if writing the output to stdout
IncludePackages (-P)               Whether to include external (Nuget) packages in the result. [Default='False']
OutputType (-T)                    Type of the output. [Default='Html']
                                   Html - Creates an html document.
                                   Json - Creates a JSON file.
                                   Xml - Creates a XML file.
                                   ConsoleJson - Writes JSON output to stdout
                                   ConsoleXml - Writes XML output to stdout
IncludeProjectNamespaces (-IPrN)   Comma separated list of project file prefixes to include. Wildcards not allowed.
                                   Only the filename is considered, case insensitive. Ex:'MyApp.Core,
                                   MyApp.Extensions' Includes only projects starting with MyApp.Core and projects
                                   starting with MyApp.Extensions [Default='']
ExcludeProjectNamespaces (-EPrN)   Comma separated list of project file prefixes to exclude. Wildcards not allowed.
                                   Only the filename is considered, case insensitive. This must be a subset of
                                   includes to be useful. Ex: 'MyApp.Extensions, MyApp.Helpers' Excludes projects
                                   starting with MyApp.Extensions and projects starting with MyApp.Helpers
IncludePackageNamespaces (-IPaN)   Comma separated list of package name prefixes to include. Wildcards not allowed.
                                   Only the package name is considered, case insensitive. If specified,
                                   'IncludePackages' is overridden to True. Ex: 'Xamarin, Microsoft' includes only
                                   packages starting with Xamarin and packages starting with Microsoft [Default='']
ExcludePackageNamespaces (-EPaN)   Comma separated list of package name prefixes to exclude. Wildcards not allowed.
                                   Only the filename is considered, case insensitive. If specified,
                                   'IncludePackages' is overridden to True. This must be a subset of includes to be
                                   useful. Ex: 'Microsoft.Logging, Azure' Excludes packages starting with
                                   Microsoft.Logging and packages starting with Azure
````

# Privacy and Security Note

In the output, the full path to project files is used as the unique identifier. So your file structure is exposed in the generated output. It attempts to only use the subdirectory structure, so an attempt is made to hide the full path.

Keep this in mind and inspect the output if you're distributing the outputs from this tool.

# Limitations

- Currently only traverses `csproj` files. No other file types are supported.
- No compile results are inspected. Only the project structure is used.

# License 
 [MIT License](https://github.com/madushans/DependenSee/blob/main/LICENSE)

 # Powered by (Thanks)

 - [PowerArgs](https://github.com/adamabdelhamed/PowerArgs)
 - [Vis.JS](https://visjs.org/)

