# DependenSee
Dotnet project and package dependency visualizer.

# Install

 ***Requires .NET 5 or higher***

`dotnet tool install DependenSee --global`

# Uninstall
`dotnet tool uninstall DependenSee --global`

# How to Run

`DependenSee root/of/projects path/to/output.html`


## Sample Output (HTML)

![path/to/](./sample-output.png)



# Docs
For full docs run without arguments
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

