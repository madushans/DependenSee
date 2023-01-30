namespace DependenSee.Core.Parsers;

/*
* SAMPLE SOLUTION file is expected to be as below.
* We are interested in the Project(... lines
* 
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.4.33205.214
MinimumVisualStudioVersion = 10.0.40219.1
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "DependenSee", "DependenSee\DependenSee.csproj", "{6C4DA9BF-3DEC-4794-8E63-3F066A0C7361}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "DependenSee.Core", "DependenSee.Core\DependenSee.Core.csproj", "{DD96A398-5B27-4D78-9F0C-1D0F516FADB1}"
EndProject
Global
	...
EndGlobal
 */


/// <summary>
/// Representation of a solution file
/// </summary>
/// <param name="Name">Name of the solution. This is usually the filename without the extension</param>
/// <param name="Id">
/// Uniquely identifies this solution in the result and is used to draw the references. 
/// </param>
/// <param name="Path">Path of this solution file relative to <see cref="DiscoveryRequest.SourceFolder"/></param>
/// <param name="UnnestedProjectIds">Other projects this solution file references</param>
/// <param name="SolutionFolders">Solution folders found in this solution file</param>
public record class SolutionFile(string Name,
                                 string Id,
                                 string Path,
                                 List<string> UnnestedProjectIds,
                                 List<SolutionFolder> SolutionFolders);


/// <summary>
/// Implementation should parse a given solution file,
/// and create a <see cref="SolutionFile"/>
/// </summary>
public interface ISolutionFileParser
{
    /// <summary>
    /// Implementation should parse a given solution file,
    /// and create a <see cref="SolutionFile"/>
    /// </summary>
    SolutionFile Parse(string solutionFilePath, DiscoveryRequest request);
}

internal record class ProjectReference(string Name, string Path, string Id, string Guid);

internal class SolutionFileParser : ISolutionFileParser
{
    private readonly IDiscoveryLogger logger;

    public SolutionFileParser(IDiscoveryLogger logger)
    {
        this.logger = logger;
    }

    public SolutionFile Parse(string solutionFilePath, DiscoveryRequest request)
    {
        var basePath = new FileInfo(solutionFilePath).Directory?.FullName;

        if (basePath is null)
            throw new Exception($"Could not get base path for {solutionFilePath}. This may be because " +
                $"this does not have a parent directory, which is unexpected.");
        var localizedPath = Path.GetRelativePath(relativeTo: request.SourceFolder, path: solutionFilePath);


        var lines = File.ReadAllLines(solutionFilePath);

        // first find all referenced projects.
        // this includes psudo projects that are actually nestedProjects/solution folders
        // but we don't know which is which yet.
        var projectReferences = FindProjectReferences(request.SourceFolder,
                                                      basePath,
                                                      solutionFilePath,
                                                      lines);

        var solutionFolders = FindSolutionFolders(request.SourceFolder,
                                                  solutionFilePath,
                                                  lines,
                                                  projectReferences);

        projectReferences = projectReferences.Where(p =>
            {
                var fullProjectFilePath = Path.GetFullPath(p.Path, request.SourceFolder);
                if (!File.Exists(fullProjectFilePath))
                {
                    logger.LogError($"Found referenced project '{fullProjectFilePath}' from {solutionFilePath} but this project " +
                        $"file does not exist (or does not have permission). This will not appear under this solution file. " +
                        $"If this solution file is valid, please open an issue with the solution file to investigate.");
                    return false;
                }
                return true;

            })
            .ToList();

        var unnestedProjectIds = projectReferences
            .Where(r => solutionFolders.All(f => !f.ChildProjectIds.Any(p => p == r.Id)))
            .Select(r => r.Id)
            .ToList();

        return new SolutionFile(
            Name: Path.GetFileNameWithoutExtension(solutionFilePath),
            Id: localizedPath,
            Path: localizedPath,
            UnnestedProjectIds: unnestedProjectIds,
            SolutionFolders: solutionFolders
            );
    }


    protected List<ProjectReference> FindProjectReferences(string sourceFolder,
                                                           string solutionBasePath,
                                                           string solutionFilePath,
                                                           string[] lines)
    {
        // Project line looks like below
        // Project("{GUID}") = "ProjectName", "Relative\Path.To.csproj", "{Project-GUID}"
        // EndProject
        //
        // example:
        // Project("{...}") = "DependenSee", "DependenSee\DependenSee.csproj", "{...}"
        // EndProject

        var projectLines = lines // must starts with 'Project("' and has a '='
            .Where(l => l.Trim().StartsWith("Project(\"") && l.IndexOf("=") > -1);

        var references = new List<ProjectReference>();

        foreach (var line in projectLines)
        {
            var segments  = line.Split('=');
            //this gives somethig like [Project("{...}")], ["DependenSee", "DependenSee\DependenSee.csproj", "{6C4DA9BF-3DEC-4794-8E63-3F066A0C7361}"]

            if (segments.Length < 2)
            {
                logger.LogError($"Solution file '{solutionFilePath}' has an unexpected project line '{line}'. " +
                    $"Expected Split(\"=\") to yield atleast 2 segments, but yielded less. Ignoring line. " +
                    $"If the solution file is valid, please open an issue with the solution file to investigate.");
                continue;
            }
            var projectInfo = segments[1].Split(","); // ["DependenSee"], ["DependenSee\DependenSee.csproj"], ["{...}"]
            if (projectInfo.Length <= 2) {
                logger.LogError($"Solution file '{solutionFilePath}' has an unexpected project line '{line}'. " +
                $"Expected Split(\"=\")[1].Split(\",\") to yield atleast 2 segments, but yielded less. Ignoring line. " +
                $"If the solution file is valid, please open an issue with the solution file to investigate.");
                continue;
            }
            var name = projectInfo[0].Trim().Trim('"').Trim('\'');
            var relativePath = projectInfo[1].Trim().Trim('"').Trim('\'');
            var guid = projectInfo[2].Trim().Trim('"').Trim('\'').Trim('{').Trim('}');

            var fullPath = Path.GetFullPath(relativePath, solutionBasePath);

            if (!fullPath.ToLower().StartsWith(sourceFolder.ToLower()))
            {
                logger.LogWarn($"Found referenced project '{fullPath}' from {solutionFilePath} " +
                    $"outside of provided {nameof(DiscoveryRequest.SourceFolder)}. Run DependenSee on the parent " +
                    $"folder of all your projects to prevent duplicates and/or missing projects from the output.");
            }

            var localizedPath = Path.GetRelativePath(relativeTo: sourceFolder, path: fullPath);

            references.Add(new ProjectReference(Name: name,
                                                     Path: localizedPath,
                                                     Id: localizedPath,
                                                     Guid: guid));
        }
        return references;
    }

    private List<SolutionFolder> FindSolutionFolders(string sourceFolder,
                                                     string solutionFilePath,
                                                     string[] lines,
                                                     List<ProjectReference> projects)
    {
        // Solution Folders (aka nested projects) are setup in the sln file as below.
        // they appear as regular projects, but has a mapping in below section
        // specifying which solution folder (nested project) has which real projects under it
        /*
         	GlobalSection(NestedProjects) = preSolution
		            {real project guid} = {guid for the solution folder they should appear under}
		            ...
	            EndGlobalSection
         */
        const string StartSectionToken = "GlobalSection(NestedProjects)";
        const string EndSectionToken = "EndGlobalSection";

        var solutionFoldersMap = new Dictionary<string, List<string>>();

        var sectionStartIndex = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Trim().StartsWith(StartSectionToken))
            {
                sectionStartIndex = i;
                break;
            }
        }
        if (sectionStartIndex == -1)
        {
            // no solution folders
            return new List<SolutionFolder>();
        }

        var sectionEndIndex = -1;
        for (int i = sectionStartIndex; i < lines.Length; i++)
        {
            if (lines[i].Trim().StartsWith(EndSectionToken))
            {
                sectionEndIndex = i;
                break;
            }
        }
        if (sectionEndIndex == -1)
        {
            // broken solution file?
            logger.LogError($"solution file '{solutionFilePath}' appear to be broken. " +
                $"Found solution folders start token '{StartSectionToken}', " +
                $"but not the corresponding end token '{EndSectionToken}'. Solution folders will not be populated. " +
                $"If this solution has solution folders, there may be further errors " +
                $"about missing project files. " +
                $"If the solution file is valid, please open an issue with the solution file to investigate.");
            return new List<SolutionFolder>();
        }

        var solutionFileRelativePath = Path.GetRelativePath(sourceFolder, solutionFilePath);
        var solutionFolderReferences = new List<ProjectReference>();

        for (int i = sectionStartIndex+1; i < sectionEndIndex; i++)
        {
            var originalLine = lines[i];
            var line = originalLine.Trim()
                               .Replace("{", "")
                               .Replace("}", "");
            var segments = line.Split('=');
            if (segments.Length < 2)
            {
                logger.LogError($"Solution file '{solutionFilePath}' has invalid " +
                    $"solution folder configuration line '{originalLine}'. Expected " +
                    $"Split(\"=\") to yield atleast 2 segments, but yielded less. Ignoring line. " +
                    $"If the solution file is valid, please open an issue with the solution file to investigate.");
                continue;
            }
            var projectGuid = segments[0].Trim();
            var solutionFolderGuid = segments[1].Trim();

            var solutionFolderProject = projects.FirstOrDefault(x => x.Guid == solutionFolderGuid);
            var project = projects.FirstOrDefault(x => x.Guid == projectGuid);
            var solutionFolderName = solutionFolderGuid;

            if (project is null)
            {
                logger.LogError($"Solution file '{solutionFilePath}' has invalid " +
                   $"solution folder configuration line '{line}'. Could not find project with ID '{projectGuid}'. " +
                   $"If this project exists, it will not be associated with the correct solution folder. " +
                   $"If the solution file is valid, please open an issue with the solution file to investigate.");
                continue;
            }

            if (solutionFolderProject is null)
            {
                logger.LogError($"Solution file '{solutionFilePath}' has invalid " +
                    $"solution folder configuration line '{line}'. Could not find project with ID '{solutionFolderGuid}'. " +
                    $"Will use solution folder guid '{solutionFolderGuid}' as its name. " +
                    $"If the solution file is valid, please open an issue with the solution file to investigate.");
            }
            else
            {
                solutionFolderName = solutionFolderProject.Name;
                // track solution folder project to be removed from the projects collection
                solutionFolderReferences.Add(solutionFolderProject);
            }

            if (solutionFoldersMap.ContainsKey(solutionFolderName))
            {
                solutionFoldersMap[solutionFolderName].Add(project.Id);
            }
            else
            {
                solutionFoldersMap.Add(solutionFolderName, new List<string> { project.Id });
            }
        }

        foreach (var reference in solutionFolderReferences)
        {
            projects.Remove(reference);
        }

        return solutionFoldersMap
            .Select(e => new SolutionFolder(Name: e.Key,
                                            // we want to make sure the ID is unique, since they can appear as a visualizer node
                                            Id: $"SolutionFolder:{solutionFileRelativePath}:{e.Key}",
                                            ChildProjectIds: e.Value))
            .ToList();
    }
}
 
