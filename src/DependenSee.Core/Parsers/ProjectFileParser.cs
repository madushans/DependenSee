using System.Xml;

namespace DependenSee.Core.Parsers;

/*
 SAMPLE Project File is expected to be as below

<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        ...
    </PropertyGroup>

    <ItemGroup>
        ...
    </ItemGroup>

    <ItemGroup>
        <!--
            PackageReference nodes denote packages, 
            usually from nuget, but not necessarily from nget.org.

            Instead of 'Include' attribute, some project may have an
            'Update' attribute to upgrade a reference as well.
        -->
        <PackageReference Include="SomeNuget.Package" Version="6.1.6" />

        <!--
            Reference node denotes another reference, sometimes a DLL reference.
            We're currently not processing these. Usually these are from very old
            pre-nuget era projects, where you keep the binaries in source control.

            However in .NET Framework projects a 'Reference' node is used for BCL
            assembles or other assembles from the GAC. These appear similar to 
            PackageReference nodes and we do process them.
        -->
        <Reference Include="SomeAssembly.dll">
          <HintPath>bin\Debug\net6.0\SomeAssembly.dll</HintPath>
        </Reference>
        
    </ItemGroup>

    <PropertyGroup>
        ...
    </PropertyGroup>

    <ItemGroup>
        <!--
            ProjectReference nodes denote 
            a reference to another project, 
            usually has a 'Include' attribute with a relative path 
        -->
        <ProjectReference Include="..\relative\path\to.some.csproj.or.vbproj" />
    </ItemGroup>
</Project>
 */


/// <summary>
/// Representation of an MSBuild project file
/// </summary>
/// <param name="ProjectName">Name of the project. This is usually the filename without the extension</param>
/// <param name="Id">
/// Uniquely identifies this project in the result and is used to draw the references. 
/// This ID must match the <see cref="SolutionFile.UnnestedProjectIds"/> or 
/// <see cref="SolutionFolder.ChildProjectIds"/> if this project is 
/// referenced in a given <see cref="SolutionFile"/> or <see cref="SolutionFolder"/>
/// </param>
/// <param name="Path">Path of this MSBuild file relative to <see cref="DiscoveryRequest.SourceFolder"/></param>
/// <param name="Projects">Other projects this MSBuild project file references</param>
/// <param name="Packages">Other packages this MSBuild project file references</param>
public record class ProjectFile(string ProjectName,
                                string Id,
                                string Path,
                                List<Project> Projects,
                                List<Package> Packages);

/// <summary>
/// Implementation should parse a given MSBuild file,
/// and create a <see cref="ProjectFile"/>
/// </summary>
public interface IProjectFileParser
{
    /// <summary>
    /// Implementation should parse a given MSBuild file,
    /// and create a <see cref="ProjectFile"/>
    /// </summary>
    ProjectFile Parse(string msBuildFilePath, DiscoveryRequest request);
}

internal class ProjectFileParser : IProjectFileParser
{
    private readonly IDiscoveryLogger logger;

    public ProjectFileParser(IDiscoveryLogger logger)
    {
        this.logger = logger;
    }

    public ProjectFile Parse(string msBuildFilePath, DiscoveryRequest request)
    {
        var xml = new XmlDocument();
        xml.Load(msBuildFilePath);
        var basePath = new FileInfo(msBuildFilePath).Directory?.FullName;

        if (basePath is null)
            throw new Exception($"Could not get base path for {msBuildFilePath}. This may be because this does not have a parent directory, which is unexpected.");

        return new ProjectFile(
            ProjectName: Path.GetFileNameWithoutExtension(msBuildFilePath),
            Id: Path.GetRelativePath(relativeTo: request.SourceFolder, path: msBuildFilePath),
            Path: Path.GetRelativePath(relativeTo: request.SourceFolder, path: msBuildFilePath),
            Projects: DiscoverProjectRefrences(xml,
                                                basePath,
                                                msBuildFilePath,
                                                request),

            Packages: DiscoverPackageReferences(xml,
                                                 msBuildFilePath)
            );
    }

    private List<Project> DiscoverProjectRefrences(XmlDocument xml,
                                                          string basePath,
                                                          string msBuildFilePath,
                                                          DiscoveryRequest request)
    {

        const string ProjectReference = "ProjectReference";
        const string Include = "Include";

        var projectReferenceNodes = xml.SelectNodes($"//*[local-name() = '{ProjectReference}']");
        if (projectReferenceNodes is null)
        {
            logger.LogError($"Could not resolve project references in '{msBuildFilePath}'. Please open an issue in github with this project file to resolve this.");
            return new List<Project>();
        }
        var projects = new List<Project>();

        foreach (XmlNode node in projectReferenceNodes)
        {
            var referencePath = node?.Attributes?[$"{Include}"]?.Value;
            if (referencePath is null)
            {
                logger.LogError($"Project file '{msBuildFilePath}' has a {ProjectReference} node without an '{Include}' attribute. Please open an issue in github with this project file to resolve this.");
                continue;
            }
            var fullPath = Path.GetFullPath(referencePath, basePath);

            string filename = Path.GetFileNameWithoutExtension(fullPath);

            if (!fullPath.ToLower().StartsWith(request.SourceFolder.ToLower()))
            {
                logger.LogWarn($"Found referenced project '{fullPath}' from {msBuildFilePath} outside of provided {nameof(request.SourceFolder)}. Run DependenSee on the parent folder of all your projects to prevent duplicates and/or missing projects from the output.");
            }

            // As a privacy measure, we attempt to remove the SourceFolder prefix.
            // This should remove any personally identifyable information (PII) in the path
            // like usernames, OS specific directory structures (like C:\) from the IDs
            var localizedPath = Path.GetRelativePath(relativeTo: request.SourceFolder, path: fullPath);

            projects.Add(new Project(Id: localizedPath,
                                          Path: localizedPath,
                                          Name: filename));
        }
        return projects;
    }

    private List<Package> DiscoverPackageReferences(XmlDocument xml,
                                                          string msBuildFilePath)
    {
        const string PackageReference = "PackageReference";
        const string Reference = "Reference";
        const string Include = "Include";
        const string Update = "Update";

        // PackageReference = Nuget package
        // Reference = COM/DLL reference. These can have a child <HintPath>relative path to dll</HintPath>'
        // Reference also present for .NET Framework projects when they reference BCL assemblies, but these
        // do not include a HintPath
        var packageReferenceNodes = xml.SelectNodes($"//*[local-name() = '{PackageReference}' or local-name() = '{Reference}']");
        var packages = new List<Package>();

        if (packageReferenceNodes is null)
        {
            logger.LogError($"Could not resolve package references in '{msBuildFilePath}'. " +
                $"Please open an issue in github with this project file to investigate this issue.");
            return packages;
        }

        foreach (XmlNode node in packageReferenceNodes)
        {
            var packageName = node?.Attributes?[$"{Include}"]?.Value
                           ?? node?.Attributes?[$"{Update}"]?.Value;

            if (packageName is null)
            {
                logger.LogError($"Project file '{msBuildFilePath}' has a '{PackageReference}' node " +
                    $"or a '{Reference}' without an '{Include}' or '{Update}' attribute. " +
                    $"Please open an issue in github with this project file to investigate this issue.");
                continue;
            }

            // we can also read the version using node?.Attributes?["Version"]?.Value;
            // however this opens a set of problems I'm not willing to deal with yet.
            // namely each project could require a different version of the same package,
            // or request an 'Update' of its version, or it could be updated via a build.props file.
            // The version could also be a floating version pattern.
            // see https://learn.microsoft.com/en-us/nuget/concepts/dependency-resolution#floating-versions
            // While we can probably spend time and effort to provide this information, replicating
            // nuget's dependency resolution algorithm is not productive for us.

            // Especially since this is mainly useful to consolidate versions and it is done well
            // by Visual Studio's Right click menu of a Solution > "Manage Nuget Packages For Solution"
            // option and selecting "Consolidate" tab on the solution.
            // Edge case is to have multiple solutions that has different projects on different
            // solutions, but this is unlikely to be much of a problem, since while is may be a bit
            // time consuming, each solution can be opened and versions consolidated.

            packages.Add(new Package(Id: packageName,
                                          Name: packageName));
        }
        return packages;
    }
}

