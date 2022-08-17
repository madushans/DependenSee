namespace DependenSee;

public class ReferenceDiscoveryService
{
    public string SourceFolder { get; set; }
    public string OutputPath { get; set; }
    public bool IncludePackages { get; set; }
    public OutputTypes OutputType { get; set; }
    public string IncludeProjectNamespaces { get; set; }
    public string ExcludeProjectNamespaces { get; set; }
    public string IncludePackageNamespaces { get; set; }
    public string ExcludePackageNamespaces { get; set; }
    public bool FollowReparsePoints { get; set; }
    public string ExcludeFolders { get; set; }
    public string SolutionFiles { get; set; }
    public bool UseSingleSolutionFile { get; set; }

    private string[] _includeProjectNamespaces { get; set; }
    private string[] _excludeProjectNamespaces { get; set; }
    private string[] _includePackageNamespaces { get; set; }
    private string[] _excludePackageNamespaces { get; set; }

    private string[] _solutionFileNames { get; set; }

    private bool _shouldIncludePackages { get; set; }

    public DiscoveryResult Discover()
    {
        var result = new DiscoveryResult
        {
            Packages = new List<Package>(),
            Projects = new List<Project>(),
            References = new List<Reference>()
        };

        _includeProjectNamespaces = ParseStringToLowercaseStringArray(IncludeProjectNamespaces);
        _excludeProjectNamespaces = ParseStringToLowercaseStringArray(ExcludeProjectNamespaces);

        _includePackageNamespaces = ParseStringToLowercaseStringArray(IncludePackageNamespaces);
        _excludePackageNamespaces = ParseStringToLowercaseStringArray(ExcludePackageNamespaces);

        _solutionFileNames = ParseStringToLowercaseStringArray(SolutionFiles);

        if (!_includeProjectNamespaces.Any()) _includeProjectNamespaces = new[] { "" };
        if (!_includePackageNamespaces.Any()) _includePackageNamespaces = new[] { "" };
        _shouldIncludePackages = IncludePackages
            || !string.IsNullOrWhiteSpace(IncludePackageNamespaces)
            || !string.IsNullOrWhiteSpace(ExcludePackageNamespaces);

        if (UseSingleSolutionFile || SolutionFiles.Length > 0)
        {
            var solutionFiles = GetValidatedSolutionFiles(SourceFolder);
            if (solutionFiles != null && solutionFiles.Count > 0)
            {
                Discover(SourceFolder, solutionFiles, result);
            }
        }
        else
        {
            Discover(SourceFolder, result);
        }

        return result;
    }

    private static string[] ParseStringToLowercaseStringArray(string list) =>
        string.IsNullOrWhiteSpace(list)
        ? Array.Empty<string>()
        : list.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(e => e.Trim().ToLower())
            .ToArray();

    private List<string> GetValidatedSolutionFiles(string folder)
    {
        var solutionFilesInFolder = GetSolutionFilesInFolder(folder);

        if (solutionFilesInFolder.Count == 0)
        {
            Console.Error.WriteLine($"No solution files found in '{folder}'\r\n\r\n");
            return null;
        }

        if (solutionFilesInFolder.Count > 1 && UseSingleSolutionFile)
        {
            Console.Error.WriteLine($"Multiple solution files found in '{folder}'\r\n\r\n");
            return null;
        }


        if (UseSingleSolutionFile)
        {
            return new List<string> { solutionFilesInFolder[0] };
        }

        var foundSolutionFiles = (from sfif in solutionFilesInFolder
                                  from sf in _solutionFileNames
                                  where sfif.EndsWith(sf, StringComparison.OrdinalIgnoreCase)
                                  select sfif).ToList();

        if (foundSolutionFiles.Count != _solutionFileNames.Length)
        {
            Console.Error.WriteLine($"Solution files specified in SolutionFiles parameter were not all found\r\n\r\n");
            return null;
        }

        return foundSolutionFiles;
    }

    private List<String> GetSolutionFilesInFolder(string folder)
    {
        var info = new DirectoryInfo(folder);
        return (from fileInfo in info.GetFiles("*.sln")
                select fileInfo.FullName).ToList();
    }

    private void Discover(string folder, IEnumerable<string> solutionFiles, DiscoveryResult result)
    {
        foreach (var solutionFile in solutionFiles)
        {
            var cleanedSolutionFileName = solutionFile.Replace(folder, ".");
            Console.WriteLine($"Discovering projects in {cleanedSolutionFileName}");
        }
    }

    private void Discover(string folder, DiscoveryResult result)
    {
        var info = new DirectoryInfo(folder);

        var excludedByRule = GetFolderExclusionFor(info.FullName);
        if (excludedByRule != null)
        {
            Console.Error.WriteLine($"Skipping folder '{folder}' excluded by rule '{excludedByRule}'\r\n\r\n");
            return;
        }

        if (!info.Exists)
        {
            Console.Error.WriteLine($"Skipping scan for missing folder '{folder}'\r\n\r\n");
            return;
        }
        if (info.Attributes.HasFlag(FileAttributes.ReparsePoint) && !FollowReparsePoints)
        {
            Console.Error.WriteLine($"Skipping scan for reparse point '{folder}'. Set {nameof(PowerArgsProgram.FollowReparsePoints)} flag to follow.\r\n\r\n");
            return;
        }

        var projectFiles = Directory.EnumerateFiles(folder, "*.csproj")
            .Concat(Directory.EnumerateFiles(folder, "*.vbproj"));
        foreach (var file in projectFiles)
        {
            var id = file.Replace(SourceFolder, "");
            var name = Path.GetFileNameWithoutExtension(file);
            if (!_includeProjectNamespaces.Any(i => name.ToLower().StartsWith(i))) continue;
            if (_excludeProjectNamespaces.Any(i => name.ToLower().StartsWith(i))) continue;

            // add this project.
            if (!result.Projects.Any(p => p.Id == id))
                result.Projects.Add(new Project
                {
                    Id = id,
                    Name = name
                });

            var (projects, packages) = DiscoverFileReferences(file);

            projects = projects
                    .Where(p =>
                        _includeProjectNamespaces.Any(i => p.Name.ToLower().StartsWith(i))
                        && !_excludeProjectNamespaces.Any(i => p.Name.ToLower().StartsWith(i)))
                    .ToList();

            foreach (var project in projects)
            {
                if (!result.Projects.Any(p => p.Id == project.Id)) result.Projects.Add(project);

                result.References.Add(new Reference
                {
                    From = id,
                    To = project.Id
                });
            }

            if (!_shouldIncludePackages) continue;

            packages = packages.Where(p =>
            {
                return _includePackageNamespaces.Any(i => p.Name.ToLower().StartsWith(i))
                    && !_excludePackageNamespaces.Any(i => p.Name.ToLower().StartsWith(i));
            }).ToList();

            foreach (var package in packages)
            {
                if (!result.Packages.Any(p => p.Id == package.Id)) result.Packages.Add(package);

                result.References.Add(new Reference
                {
                    From = id,
                    To = package.Id
                });
            }
        }
        var directories = Directory.EnumerateDirectories(folder);
        foreach (var directory in directories)
        {
            Discover(directory, result);
        }
    }

    private (List<Project> projects, List<Package> packages) DiscoverFileReferences(string path)
    {
        var xml = new XmlDocument();
        xml.Load(path);
        var basePath = new FileInfo(path).Directory.FullName;

        var projects = DiscoverProjectRefrences(xml, basePath);
        var packages = _shouldIncludePackages
            ? DiscoverPackageReferences(xml)
            : null;

        return (projects, packages);
    }

    private List<Package> DiscoverPackageReferences(XmlDocument xml)
    {
        // PackageReference = Nuget package
        // Reference = COM/DLL reference. These can have a child <HintPath>relative path to dll</HintPath>'
        // Reference also present for .NET Framework projects when they reference BCL assemblies, but these
        // do not include a HintPath
        var packageReferenceNodes = xml.SelectNodes("//*[local-name() = 'PackageReference' or local-name() = 'Reference']");
        var packages = new List<Package>();
        foreach (XmlNode node in packageReferenceNodes)
        {
            var packageName = node.Attributes["Include"]?.Value
                           ?? node.Attributes["Update"].Value;

            packages.Add(new Package
            {
                Id = packageName,
                Name = packageName
            });
        }
        return packages;
    }

    private string GetFolderExclusionFor(string fullFolderPath)
    {
        if (string.IsNullOrWhiteSpace(ExcludeFolders)) return null;

        var allRules = ExcludeFolders
            .Split(',')
            .Select(r => Path.IsPathRooted(r) ? r : Path.GetFullPath(r, SourceFolder))
            .Select(r => r.ToLower().Trim())
            .ToList();

        fullFolderPath = fullFolderPath.ToLower();
        return allRules.FirstOrDefault(r => fullFolderPath.StartsWith(r));
    }

    private List<Project> DiscoverProjectRefrences(XmlDocument xml, string basePath)
    {
        var projectReferenceNodes = xml.SelectNodes("//*[local-name() = 'ProjectReference']");
        var projects = new List<Project>();

        foreach (XmlNode node in projectReferenceNodes)
        {
            var referencePath = node.Attributes["Include"].Value;
            var fullPath = Path.GetFullPath(referencePath, basePath);

            string filename = Path.GetFileNameWithoutExtension(fullPath);

            if (!fullPath.ToLower().StartsWith(SourceFolder.ToLower()))
            {
                Console.Error.WriteLine($"Found referenced project '{fullPath}' outside of provided source folder. Run DependenSee on the parent folder of all your projects to prevent duplicates and/or missing projects from the output.\r\n\r\n");
            }

            projects.Add(new Project
            {
                Id = fullPath.Replace(SourceFolder, ""),
                Name = filename
            });
        }
        return projects;
    }
}
