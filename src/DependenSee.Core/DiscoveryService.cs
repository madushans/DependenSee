using DependenSee.Core.Parsers;

namespace DependenSee.Core;

/// <summary>
/// Entry point for discovering your dependencies.
/// </summary>
public class DiscoveryService
{
    private readonly IProjectFileParser msBuildFileParser;
    private readonly ISolutionFileParser solutionFileParser;
    private readonly IDiscoveryLogger logger;

    /// <summary>
    /// Constructs a new <see cref="DiscoveryService"/> that can 
    /// discover the dependencies of your solution.
    /// </summary>
    /// <param name="msBuildFileParser">
    /// Implementation that can parse an MSBuild file. 
    /// Leave as <see langword="null"/> to use the default implementation
    /// of <see cref="ProjectFileParser"/>
    /// </param>
    /// <param name="solutionFileParser">
    /// Implementation that can parse a solution file. 
    /// Leave as <see langword="null"/> to use the default implementation
    /// of <see cref="SolutionFileParser"/>
    /// </param>
    /// <param name="logger">
    /// Implementation of a logger. Leaving as <see langword="null"/> will
    /// use the <see cref="NullDiscoveryLogger"/> which will omit logging.
    /// </param>
    public DiscoveryService(IProjectFileParser? msBuildFileParser = null,
                            ISolutionFileParser? solutionFileParser = null,
                            IDiscoveryLogger? logger = null)
    {
        this.logger = logger ?? new NullDiscoveryLogger();
        this.msBuildFileParser = msBuildFileParser  ?? new ProjectFileParser(this.logger);
        this.solutionFileParser = solutionFileParser ?? new SolutionFileParser(this.logger);
    }

    /// <summary>
    /// Discovers dependencies in a solution folder.
    /// </summary>
    /// <param name="request"></param>
    /// <exception cref="ArgumentNullException"><see cref="DiscoveryRequest.SourceFolder"/> must be specified</exception>
    public DiscoveryResult Discover(DiscoveryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SourceFolder))
            throw new ArgumentException($"{nameof(request)}.{nameof(request.SourceFolder)} must be provided");

        if (request.ExcludeFolders is null) 
            request  = request with { ExcludeFolders = Array.Empty<string>() };
        
        var result = new DiscoveryResult();
        Discover(request.SourceFolder, request, result);
        return result;
    }

    private void Discover(string folder, DiscoveryRequest request, DiscoveryResult result)
    {
        var info = new DirectoryInfo(folder);

        var exclusion = GetFolderExclusionFor(info.FullName, request);
        if (exclusion is not null)
        {
            logger.LogInfo($"Skipping folder '{folder}' excluded by '{exclusion}'");
            return;
        }

        if (!info.Exists)
        {
            logger.LogWarn($"Skipping discovery for missing folder or folder without permission '{folder}'");
            return;
        }
        if (info.Attributes.HasFlag(FileAttributes.ReparsePoint) && !request.FollowReparsePoints)
        {
            logger.LogWarn($"Skipping discovery for reparse point '{folder}'. " +
                $"Set {nameof(request.FollowReparsePoints)} flag to follow reparse points.");
            return;
        }

        DiscoverProjectFiles(request, result, folder);
        DiscoverSolutionFiles(request, result, folder);

        var directories = Directory.EnumerateDirectories(folder);
        foreach (var directory in directories)
        {
            Discover(directory, request, result);
        }
    }

    private void DiscoverProjectFiles(DiscoveryRequest request, DiscoveryResult result, string folder)
    {
        var projectFiles = Directory.EnumerateFiles(folder, "*.csproj")
                                    .Concat(Directory.EnumerateFiles(folder, "*.vbproj"))
                                    .ToList();
        foreach (var file in projectFiles)
        {
            var msBuildFile = msBuildFileParser.Parse(file, request);

            // add this project.
            if (!result.Projects.Any(p => p.Id == msBuildFile.Id))
                result.Projects.Add(new Project(Id: msBuildFile.Id,
                                                     Name: msBuildFile.ProjectName,
                                                     Path: msBuildFile.Path));

            foreach (var project in msBuildFile.Projects)
            {
                if (!result.Projects.Any(p => p.Id == project.Id))
                {
                    result.Projects.Add(project);
                }

                result.References.Add(new Reference(From: msBuildFile.Id, To: project.Id));
            }

            foreach (var package in msBuildFile.Packages)
            {
                if (!result.Packages.Any(p => p.Id == package.Id))
                {
                    result.Packages.Add(package);
                }

                result.References.Add(new Reference(From: msBuildFile.Id, To: package.Id));
            }
        }
    }

    private void DiscoverSolutionFiles(DiscoveryRequest request, DiscoveryResult result, string folder)
    {
        var solutionFiles = Directory.EnumerateFiles(folder, "*.sln")
                                     .ToList();

        foreach (var file in solutionFiles)
        {
            var solutionFile = solutionFileParser.Parse(file, request);

            if (result.Solutions.Any(s => s.Id == solutionFile.Id))
            {
                logger.LogWarn($"Detected {solutionFile.Path} more than once during discovery. " +
                    $"This may be a sign of loops in the file system structure. Consider excluding " +
                    $"folders/reparse points/junctions/symlinks that has loops with {nameof(request.ExcludeFolders)} option.");
                continue;
            }

            result.Solutions.Add(new Solution(Id: solutionFile.Id,
                                                   Name: solutionFile.Name,
                                                   Path: solutionFile.Path,
                                                   UnnestedProjectIds: solutionFile.UnnestedProjectIds,
                                                   SolutionFolders: solutionFile.SolutionFolders));
        }
    }


    private string? GetFolderExclusionFor(string fullFolderPath, DiscoveryRequest request)
    {
        if (request.ExcludeFolders is null || !request.ExcludeFolders.Any()) return null;

        var allRules = request.ExcludeFolders
            .Select(r => Path.IsPathRooted(r) ? r : Path.GetFullPath(r, request.SourceFolder))
            .Select(r => r.ToLower().Trim()) // case insensitive. May cause issues on *nix systems
            .ToList();

        fullFolderPath = fullFolderPath.ToLower();
        return allRules.FirstOrDefault(r => fullFolderPath.StartsWith(r));
    }
}

