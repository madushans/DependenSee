using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DependenSee.Api
{
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
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "DependenSee.Api", "DependenSee.Api\DependenSee.Api.csproj", "{DD96A398-5B27-4D78-9F0C-1D0F516FADB1}"
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
    /// <param name="ProjectIds">Other projects this solution file references</param>
    public record class SolutionFile(string Name,
                                     string Id,
                                     string Path,
                                     List<string> ProjectIds);

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
                throw new Exception($"Could not get base path for {solutionFilePath}. This may be because this does not have a parent directory, which is unexpected.");
            var localizedPath = Path.GetRelativePath(relativeTo: request.SourceFolder, path: solutionFilePath);
            return new SolutionFile(
                Name: Path.GetFileNameWithoutExtension(solutionFilePath),
                Id: localizedPath,
                Path: localizedPath,
                ProjectIds: DiscoverProjectIds(basePath,
                                           solutionFilePath,
                                           request)
                );
        }

        private List<string> DiscoverProjectIds(string basePath,
                                               string solutionFilePath,
                                               DiscoveryRequest request)
        {

            var projectIds = new List<string>();
            var lines = File.ReadAllLines(solutionFilePath);

            // Project line looks like below
            // Project("{GUID}") = "ProjectName", "Relative\Path.To.csproj", "{CONFIG-GUID}"
            // EndProject
            //
            // example:
            // Project("{...}") = "DependenSee", "DependenSee\DependenSee.csproj", "{...}"
            // EndProject
            
            var references = lines
                // starts with 'Project("' and has a '='
                .Where(l => l.Trim().StartsWith("Project(\"") && l.IndexOf("=") > -1)
                .Select(l => l.Split("=")) // [Project("{...}")], ["DependenSee", "DependenSee\DependenSee.csproj", "{6C4DA9BF-3DEC-4794-8E63-3F066A0C7361}"]
                .Where(parts => parts.Length > 1) // has 2 parts
                .Select(parts => parts[1]) // "DependenSee", "DependenSee\DependenSee.csproj", "{...}"
                .Select(s => s.Split(",")) // ["DependenSee"], ["DependenSee\DependenSee.csproj"], ["{...}"]
                .Where(e => e.Length > 1) // has name and path
                .Select(e => (e[1].Trim().Trim('"').Trim('\''))) // get path
                .ToList();

            foreach (var path in references)
            {
                var fullPath = Path.GetFullPath(path, basePath);
                string filename = Path.GetFileNameWithoutExtension(fullPath);

                if (!fullPath.ToLower().StartsWith(request.SourceFolder.ToLower()))
                {
                    logger.LogWarn($"Found referenced project '{fullPath}' from {solutionFilePath} " +
                        $"outside of provided {nameof(request.SourceFolder)}. Run DependenSee on the parent " +
                        $"folder of all your projects to prevent duplicates and/or missing projects from the output.");
                }

                if(!File.Exists(fullPath))
                {
                    logger.LogWarn($"Found referenced project '{fullPath}' from {solutionFilePath} but this project " +
                        $"file does not exist (or does not have permission). This will not appear under this solution file. " +
                        $"If this solution file is valid, please open an issue with the solution file to investigate.");
                    continue;
                }

                // As a privacy measure, we attempt to remove the SourceFolder prefix.
                // This should remove any personally identifyable information (PII) in the path
                // like usernames, OS specific directory structures (like C:\) from the IDs
                var localizedPath = Path.GetRelativePath(relativeTo: request.SourceFolder, path: fullPath);

                projectIds.Add(localizedPath);
            }


            return projectIds;
        }
    }
}
     
