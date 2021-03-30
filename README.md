# DependenSee
Dotnet project and package dependency visualizer.

# Install

 ***Requires .NET 5 or higher***

`dotnet tool install DependenSee --global`

available on [Nuget](https://www.nuget.org/packages/DependenSee) 

![Nuget Badge](https://buildstats.info/nuget/dependensee)

# Uninstall
`dotnet tool uninstall DependenSee --global`

# How to Run

`DependenSee root/of/projects path/to/output.html`


## Sample Output (HTML)

![DependenSee sample html output](https://raw.githubusercontent.com/madushans/DependenSee/main/sample-output.png)


# Why `DependenSee` over 'X'

Current popular options are to either use NDepend, VS Architecture Explorer or Code Map. While these options are feature rich, they also has a licensing cost. If all you want is to see the dependency graph, and nothing else, it may be hard to justify the licensing cost. Then `DependenSee` is for you.

If you need to see the Type structure, relationships between your methods or types .etc. then you should use one of the options above instead. DependenSee is mean to be very simple, easy, straight forward to use and FREE! `DependenSee` does not intend to compete with the above. See [Limitations](#Limitations)

# Features

- Creates the dependency graph for your solution.
- Can only include or exclude certain namespaces so the result is not overwhelming or filled with noise.
- Can create HTML, XML, JSON outputs
- Can return XML or JSON to `STDOUT` for further processing by other command line tools
 
For full docs run without any arguments
`DependenSee`


# Documentation

`Usage - DependenSee <SourceFolder> [<OutputPath>] -options`

## Help

Shows help descriptions. Help is also displayed if no arguments were provided.

**Shorthand: `-H`**

### Examples
- `DependenSee -Help`
- `DependenSee -H`
- `DependenSee`

## SourceFolder [Required]

Root folder (usually solution folder) to look for csproj files recursively. If no explicit options are specified, the first argument is assumed to be this option.

If your path has spaces, you will need to enclose it in double quotes.

### Examples

- `DependenSee \Source\MySolutionFolder` 
- `DependenSee -SourceFolder \Source\MySolutionFolder`
- `DependenSee -S \Source\MySolutionFolder`

## OutputPath

Path to write the result. Not required if writing the output to stdout.
If no  explicit options are specified, the second argument is assumed to be this option.

If your path has spaces, you will need to enclose it in double quotes.

**Shorthand: `-O`**

### Examples

- `DependenSee \Source\SolutionFolder \Test\MyOutput.html`
- `DependenSee \Source\SolutionFolder -O \Test\MyOutput.html`
- `DependenSee \Source\SolutionFolder -OutputPath \Test\MyOutput.html`

## IncludePackages

Whether to include external (Nuget) packages in the result.

**Shorthand: `-P`**

**Default: `False`**


### Examples 

- `DependenSee \Source\SolutionFolder \Test\MyOutput.html -P`
- `DependenSee \Source\SolutionFolder \Test\MyOutput.html -IncludePackages`

## OutputType

Type of output to produce. Following types are available.

- `Html` - Creates an html document.
- `Json` - Creates a JSON file.
- `Xml` - Creates a XML file.
- `ConsoleJson` - Writes JSON output to stdout
- `ConsoleXml` - Writes XML output to stdout

When a `Console...` type output is  specified, the `-OutputPath` can be ommitted.

**Shorthand: `-T`**

**Default: `Html`**

### Examples

- `DependenSee \Source\SolutionFolder \Test\MyOutput.html -T Xml`
- `DependenSee \Source\SolutionFolder \Test\MyOutput.html -OutputType Json`
- `DependenSee \Source\SolutionFolder -T ConsoleJson`

## IncludeProjectNamespaces
Comma separated list of project file prefixes to include. Wildcards not allowed. Only the filename is considered, case insensitive. 

If you want to include spaces between items, make sure you enclose the parameter value in double quotes.

**Shorthand: `-IPrN`**

**Default: `<empty string>`**

### Examples

- `DependenSee \Source\SolutionFolder -O ConsoleJson -IncludeProjectNamespaces MyApp.Extensions,MyApp.Core`
  -  Includes only projects starting with MyApp.Core and projects starting with MyApp.Extensions
- `DependenSee \Source\SolutionFolder -O ConsoleJson -IPrN MyApp.Extensions,MyApp.Core`
  -  Includes only projects starting with MyApp.Core and projects starting with MyApp.Extensions

## ExcludeProjectNamespaces

Comma separated list of project file prefixes to exclude. Wildcards not allowed. Only the filename is considered, case insensitive. This must be a subset of includes to be useful.

If you want to include spaces between items, make sure you enclose the parameter value in double quotes.

**Shorthand: `-EPrN`**

**Default: `<unspecified>`**

### Examples

- `DependenSee \Source\SolutionFolder -O ConsoleJson -ExcludeProjectNamespaces MyApp.Extensions, MyApp.Helpers`
  -  Excludes projects starting with MyApp.Extensions and projects starting with MyApp.Helpers
- `DependenSee \Source\SolutionFolder -O ConsoleJson -EPrN MyApp.Extensions, MyApp.Helpers`
  -  Excludes projects starting with MyApp.Extensions and projects starting with MyApp.Helpers

## IncludePackageNamespaces
Comma separated list of package name prefixes to include. Wildcards not allowed. Only the package name is considered, case insensitive. If specified, `-IncludePackages` is overridden to `True`.

If you want to include spaces between items, make sure you enclose the parameter value in double quotes.

**Shorthand: `-IPaN`**

**Default: `<empty string>`**

### Examples

- `DependenSee \Source\SolutionFolder -O ConsoleJson -IncludePackageNamespaces Xamarin,Microsoft`
  - includes **only** packages starting with Xamarin and packages starting with Microsoft
- `DependenSee \Source\SolutionFolder -O ConsoleJson -IPaN Xamarin,Microsoft`
  - includes **only** packages starting with Xamarin and packages starting with Microsoft

## ExcludePackageNamespaces

Comma separated list of package name prefixes to exclude. Wildcards not allowed. Only the filename is considered, case insensitive. If specified, `-IncludePackages` is overridden to `True`. This must be a subset of includes to be useful.

If you want to include spaces between items, make sure you enclose the parameter value in double quotes.

**Shorthand: `-EPaN`**

**Default: `<unspecified>`**

### Examples

- `DependenSee \Source\SolutionFolder -O ConsoleJson -ExcludePackageNamespaces Microsoft.Logging,Azure`
  - Excludes packages starting with Microsoft.Logging and packages starting with Azure
- `DependenSee \Source\SolutionFolder -O ConsoleJson -EPaN Microsoft.Logging,Azure`
  - Excludes packages starting with Microsoft.Logging and packages starting with Azure


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

