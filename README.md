# DependenSee
Dotnet project and package dependency visualizer.

---

## V3 Is Coming! 🔜

I have started work on the next major version of DependenSee. See [what's coming!](/docs/Version3.md)

---

# Install

 ***Requires .NET 6 Runtime***

`dotnet tool install DependenSee --global`

available on [Nuget](https://www.nuget.org/packages/DependenSee) 

![Nuget Badge](https://buildstats.info/nuget/dependensee)

## Uninstall
`dotnet tool uninstall DependenSee --global`

# Basic Usage

`DependenSee root/of/projects path/to/output.html`

See [full CLI documentation](/docs/CommandLine.md)

## Sample Output (HTML, interactive)

Download [this](https://raw.githubusercontent.com/madushans/DependenSee/main/sample-html-output.html) as html and open in a browser for a demo.

![DependenSee sample html output](https://raw.githubusercontent.com/madushans/DependenSee/main/sample-output.png)


# [Command Line Arguments > 🔗](/docs/CommandLine.md) 

# [Troubleshooting > 🔗](/docs/Troubleshooting.md) 


# Why `DependenSee` over 'X'

Current popular options are to either use NDepend, VS Architecture Explorer or Code Map. While these options are feature rich, they also has a licensing cost. If all you want is to see the dependency graph, and nothing else, it may be hard to justify the licensing cost. Then `DependenSee` is for you.

If you need to see the Type structure, relationships between your methods or types .etc. then you should use one of the options above instead. DependenSee is mean to be very simple, easy, straight forward to use and FREE! `DependenSee` does not intend to compete with the above. See [Limitations](#Limitations)

# Features

- Creates the dependency graph for your solution.
- Can only include or exclude certain namespaces so the result is not overwhelming or filled with noise.
- Can create HTML, XML, JSON and Graphviz outputs
- Can return output to `STDOUT` for further processing by other command line tools
- Returns errors and warnings to `STDERR`
 
For full docs run without any arguments
`DependenSee`


# Privacy and Security Note

In the output, the full path to project files is used as the unique identifier. So your file structure is exposed in the generated output. It attempts to only use the subdirectory structure, so an attempt is made to hide the full path, however it is possible these paths may include your username for example, if your project was located in the default VS path/repo clone location of `C:\Users\<username>\Repos\...`.

Keep this in mind and inspect the output if you're distributing the outputs from this tool.

# Limitations

- Currently only traverses `csproj` and `vbproj` files. No other file types are supported.
- No compile results are inspected. Only the project structure is used.

# License 
 [MIT License](https://github.com/madushans/DependenSee/blob/main/LICENSE)

# Support
If you are experiencing issues, please [open an issue](https://github.com/madushans/DependenSee/issues) with details and reproduction steps.

 # Contributions

 Pull requests welcome. ♥

 Please branch off of `dev` branch and put a PR to `dev` for your changes.
 If you have a contribution you're not sure about, please feel free to [open an issue](https://github.com/madushans/DependenSee/issues). However a prior approval is not necessary for a PR to be merged.

 Once approved, all pending changes (possibly multiple PRs) will be merged to `main` for a release to be distributed via NuGet.

 # Powered by (Thanks)

 - [Your Community Contributions 🙏](https://github.com/madushans/DependenSee/pulls?q=is%3Apr+is%3Aclosed)
 - [PowerArgs](https://github.com/adamabdelhamed/PowerArgs)
 - [Vis.JS](https://visjs.org/) 

