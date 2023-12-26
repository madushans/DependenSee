
# Command Line Arguments

`Usage - DependenSee <SourceFolder> [<OutputPath>] -options`

For full docs in the command line, run without any arguments.

To see the version installed, use `dotnet tool list` instead. (if you installed globally, `-g` option is required.)

## Exit code
DependenSee will exit with 0 if there were no errors (may have warnings) and with 1 if the command failed.

See `stderr` stream for warnings. If you're running in a standard command line, these will be included in the output, but will not be included if you're redirecting/piping the `stdout` to a file or another program.

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

Note that if a project inside this folder has a reference to a project that is outside of this folder, DependenSee WILL enumerate those folders. However this is not reccomended. Always run DependenSee on the root folder of your projects for consistent and correct results.

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
- `Graphviz` - Creates a Graphviz/DOT file.
- `Mermaid` - Creates a Mermaid/MMD file.
- `ConsoleJson` - Writes JSON output to stdout
- `ConsoleXml` - Writes XML output to stdout
- `ConsoleGraphviz` - Writes Graphviz output to stdout
- `ConsoleMermaid` - Writes Mermaid output to stdout

When a `Console...` type output is  specified, the `-OutputPath` can be ommitted.
`Console...` output types may still write warings to `stderr` stream. If you're piping just the `stderr` into another program, consider checking the `stderr` for warnings as well.

if you'd like to hide `stderr` output from your console output, see `Troubleshooting` section below.

To visualize Graphviz output, either use 
- Use an online visualizer such as [https://dreampuf.github.io/GraphvizOnline](https://dreampuf.github.io/GraphvizOnline)

- or use tooling from [https://graphviz.org](https://graphviz.org)
   - once installed you can use `.\dot.exe DependenSee-OutputFile.dot -oTargetFile.Svg -Tsvg`
   - `dot.exe` is located at `your-graphvis-installation/bin`
   - see Graphviz docs for [command line usage](https://graphviz.org/doc/info/command.html) and supported [output types](https://graphviz.org/doc/info/output.html) 

**Shorthand: `-T`**

**Default: `Html`**

### Examples

- `DependenSee \Source\SolutionFolder \Test\MyOutput.html -T Xml`
- `DependenSee \Source\SolutionFolder \Test\MyOutput.html -OutputType Json`
- `DependenSee \Source\SolutionFolder -T ConsoleJson`

## HtmlTitle

Document title for Html output. Ignored when creating output types other than `Html`.

If your title has spaces, you will need to enclose it in double quotes. The title should not be HTML-encoded ahead of time as DependenSee will do this automatically.

**Shorthand: `-HT`**

**Default: `DependenSee`**

### Examples

- `DependenSee \Source\SolutionFolder \Test\MyOutput.html -HtmlTitle "My Graph Title"`
- `DependenSee \Source\SolutionFolder \Test\MyOutput.html -T Html -HtmlTitle my-graph-title`
- `DependenSee \Source\SolutionFolder \Test\MyOutput.html -HT "My Graph Title"`

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

## TrimProjectNamespaces
Comma separated list of project file prefixes to trim. Wildcards not allowed. Only the filename is considered, case insensitive. 

**Shorthand: `-TPrN`**

**Default: `<empty string>`**

### Examples

- `DependenSee \Source\SolutionFolder -O ConsoleJson -TrimProjectNamespaces MyApp`
  -  Displays project names starting with MyApp. 'MyApp.Core' and 'MyApp.Extensions' display as 'Core' and 'Extensions'
- `DependenSee \Source\SolutionFolder -O ConsoleJson -TPrN MyApp`
  -  Displays project names starting with MyApp. 'MyApp.Core' and 'MyApp.Extensions' display as 'Core' and 'Extensions'

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

Comma separated list of package name prefixes to exclude. Wildcards not allowed. Only the package name is considered, case insensitive. If specified, `-IncludePackages` is overridden to `True`. This must be a subset of includes to be useful.

If you want to include spaces between items, make sure you enclose the parameter value in double quotes.

**Shorthand: `-EPaN`**

**Default: `<unspecified>`**

## TrimPackageNamespaces
Comma separated list of package name prefixes to trim. Wildcards not allowed. Only the package name is considered, case insensitive. If specified, `-IncludePackages` is overridden to `True`.

**Shorthand: `-TPaN`**

**Default: `<empty string>`**

### Examples

- `DependenSee \Source\SolutionFolder -O ConsoleJson -TrimPackageNamespaces MyApp`
  -  Displays package names starting with MyApp. 'MyApp.Core' and 'MyApp.Extensions' display as 'Core' and 'Extensions'
- `DependenSee \Source\SolutionFolder -O ConsoleJson -TPaN MyApp`
  -  Displays package names starting with MyApp. 'MyApp.Core' and 'MyApp.Extensions' display as 'Core' and 'Extensions'

## FollowReparsePoints

Set if you want the scan to follow valid reparse points. This is helpful if your project references are relying on symlinks, NTFS junction points .etc. 

If set to `True` and a junction/reparse point points to a folder that does not exist (`DirectoryInfo.Exists == false`) it is still skipped (with a message to `stderr`).

If set to `False` and a reparse point is encountered, a message is also emitted with the full path showing which reparse point is skipped.

**Shorthand: `-FReP`**

**Default: `False`**

### Examples
- `DependenSee \Source\SolutionFolder -FollowReparsePoints`
- `DependenSee \Source\SolutionFolder -FReP`

## ExcludeFolders

Comma Separated list of folders (either absolute paths or relative to `SourceFolder`) to skip during scan, even if there are references to them from your projects. Wildcards not allowed. When a folder is matched and skipped by a rule specified this way, will emit a warning into the `stderr` stream. **Paths here are  case insensitive**.

While you can specify to exclude a path outside of the source folder, and it will be considered if a project has a reference to a project outside of the source folder, this is not recommended. Always run DependenSee against the root folder of your projects to get consitent and correct results.

**Shorthand: `-EFol`**

**Default: `<unspecified>`**

### Examples

- `DependenSee \Source\SolutionFolder -ExcludeFolders "\Source\SolutionFolder\docs, Source\SolutionFolder\clientapp\node-modules"`  
- `DependenSee \Source\SolutionFolder -EFol "C:\Source\SolutionFolder\docs, \Source\SolutionFolder\clientapp\node-modules"`
