namespace DependenSee.Core;

/// <summary>
/// Represents a request details for Discovery
/// </summary>
public record class DiscoveryRequest
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Root folder where the discovery should start. This is required.
    /// </summary>
    public string SourceFolder { get; init; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// List of folder paths that should be excluded from discovery run.
    /// This can be used if you have a folder that does not have MSBuild projects and should not be inspected, or to avoid folder(s)
    /// that DependenSee has issues inspecting.
    /// Note that if a project in a folder that is not excluded, has a reference to a project that is in an excluded folder, 
    /// it will be still be included. This only excludes enumerating these folders for MSBuild files.
    /// </summary>
    public string[]? ExcludeFolders { get; init; } = null;

    /// <summary>
    /// Whether to follow filesystem reparse points. These include but not limited to NTFS junction points and symlinks.
    /// By default these are not followed since this can cause the discovery to follow filesystem loops.
    /// If you have reparse points you would like to follow, but some of them has loops that should not be followed, 
    /// you can turn this on, and exclude the ones that has loops with <see cref="ExcludeFolders"/>
    /// 
    /// One known example is when your solution folder contains node_modules managed by pnpm.
    /// See https://github.com/madushans/DependenSee/issues/20
    /// </summary>
    public bool FollowReparsePoints { get; init; } = false;
}


/// <summary>
/// Result of a discovery run
/// </summary>
public class DiscoveryResult
{
    /// <summary>
    /// Details of a discovered projects.
    /// </summary>
    public List<Project> Projects { get; } = new List<Project>();

    /// <summary>
    /// Details of a discovered packages that are referenced by discovered <see cref="Projects"/>.
    /// </summary>
    public List<Package> Packages { get; } = new List<Package>();

    /// <summary>
    /// Details of how projects and packages are referenced
    /// </summary>
    public List<Reference> References { get; } = new List<Reference>();

    /// <summary>
    /// Details of how discovered solutions and what projects they reference.
    /// </summary>
    public List<Solution> Solutions { get; } = new List<Solution>();

    // we can also gather and include all the logs accumilated during the discovery
    // however logs can contain full paths, which may contain PII like usernames, OS
    // specific file structures. If we're doing this, it should be an explicit opt-in.
}


/// <param name="Id">
/// Unique identifier for this project. This is 
/// generated and serves only to uniquely identify this project. 
/// This is usually a partial/relative file path, however it is not
/// guaranteed to be so. If you want the path, use <paramref name="Path"/>.
/// </param>
/// <param name="Name">
/// Name of the project, usually derived from the MSBuild filename.
/// </param>
/// <param name="Path">
/// Relative path of the project file, relative to the 
/// <see cref="DiscoveryRequest.SourceFolder"/>
/// </param>
public record class Project(string Id, string Name, string Path);


/// <param name="Id">
/// Unique identifier for this package. This is usually the 
/// package name, but is not guaranteed to be so.
/// </param>
/// <param name="Name">
/// Name of the package.
/// </param>
public record class Package(string Id, string Name);


/// <summary>
/// Description of a reference between entities. These can be 
/// projects, packages, solutions or solution folders
/// </summary>
/// <param name="From">
/// Reference origination.
/// </param>
/// <param name="To">
/// Reference destination.
/// </param>
public record class Reference(string From, string To);


/// <param name="Id">
/// Unique identifier for this solution. 
/// This is usually a partial/relative file path, however it is not
/// guaranteed to be so. If you want the path, use <paramref name="Path"/>.
/// </param>
/// <param name="Name">
/// Name of the solution, usually derived from the solution filename.
/// </param>
/// <param name="Path">
/// Relative path of the solution file, relative to the 
/// <see cref="DiscoveryRequest.SourceFolder"/>
/// </param>
/// <param name="UnnestedProjectIds">
/// IDs of projects this solution references, 
/// but is not in a solution folder
/// </param>
/// <param name="SolutionFolders">
/// Solution folders in this solution and the 
/// IDs of projects under them
/// </param>
public record class Solution(string Id,
                             string Name,
                             string Path,
                             List<string> UnnestedProjectIds,
                             List<SolutionFolder> SolutionFolders);

/// <param name="Name">Name of the solution folder</param>
/// <param name="Id">Unique identifier for this solution folder. This does not 
/// match to any physical path since it is a virtual folder.</param>
/// <param name="ChildProjectIds">IDs of the child projects under this solution folder</param>
public record class SolutionFolder(string Name,
                                       string Id,
                                       List<string> ChildProjectIds);