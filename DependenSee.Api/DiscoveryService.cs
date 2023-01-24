using System.Diagnostics;
using System.Xml;
using Microsoft.CSharp;

namespace DependenSee.Api;

public class DiscoveryService
{
    private readonly IMsBuildFileParser msBuildFileParser;
    private readonly IDiscoveryLogger logger;

    public DiscoveryService(IMsBuildFileParser? msBuildFileParser = null, IDiscoveryLogger? logger = null)
    {
        this.logger = logger ?? new NullDiscoveryLogger();
        this.msBuildFileParser = msBuildFileParser  ?? new MsBuildFileParser(this.logger);
    }
    public DiscoveryResult Discover(DiscoveryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SourceFolder))
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.SourceFolder)}");

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
            logger.LogWarn($"Skipping scan for missing folder '{folder}'");
            return;
        }
        if (info.Attributes.HasFlag(FileAttributes.ReparsePoint) && !request.FollowReparsePoints)
        {
            logger.LogWarn($"Skipping scan for reparse point '{folder}'. Set {nameof(request.FollowReparsePoints)} flag to follow reparse points.");
            return;
        }

        var projectFiles = Directory.EnumerateFiles(folder, "*.csproj")
                                    .Concat(Directory.EnumerateFiles(folder, "*.vbproj"))
                                    .ToList();

        foreach (var file in projectFiles)
        {
            // As a privacy measure, we attempt to remove the SourceFolder prefix.
            // This should remove any personally identifyable information (PII) in the path
            // like usernames, OS specific directory structures (like C:\) from the IDs
            var id = Path.GetRelativePath(relativeTo: request.SourceFolder, path: file);
            var name = Path.GetFileNameWithoutExtension(file);
            
            var msBuildFile = msBuildFileParser.Parse(file, request);

            // add this project.
            if (!result.Projects.Any(p => p.Id == id))
                result.Projects.Add(new Project(Id: id, Name: name));

            foreach (var project in msBuildFile.Projects)
            {
                if (!result.Projects.Any(p => p.Id == project.Id)) result.Projects.Add(project);

                result.References.Add(new Reference(From: id,To: project.Id));
            }

            foreach (var package in msBuildFile.Packages)
            {
                if (!result.Packages.Any(p => p.Id == package.Id)) result.Packages.Add(package);

                result.References.Add(new Reference(From: id, To: package.Id));
            }
        }
        var directories = Directory.EnumerateDirectories(folder);
        foreach (var directory in directories)
        {
            Discover(directory, request, result);
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

