namespace DependenSee.Tests;

public class ReferenceDiscoveryServiceTests
{
    private readonly ITestOutputHelper _output;

    public ReferenceDiscoveryServiceTests(ITestOutputHelper output) =>
        _output = output;

    [Fact]
    public void Can_discover_with_default_settings()
    {
        var binPath = Path.GetDirectoryName(typeof(ReferenceDiscoveryServiceTests).Assembly.Location);
        var projPath = Path.GetFullPath(Path.Combine(binPath!, "../../../"));

        _output.WriteLine(projPath);

        var referenceDiscovery = new ReferenceDiscoveryService
        {
            SourceFolder = projPath
        };

        var discoveryResult = referenceDiscovery.Discover();

        discoveryResult.Projects.Should().HaveCountGreaterThanOrEqualTo(2);
        discoveryResult.Projects.Should().Contain(x => x.Name == "DependenSee");
        discoveryResult.Projects.Should().Contain(x => x.Name == "DependenSee.Tests");

        discoveryResult.References.Should().HaveCountGreaterThanOrEqualTo(1);
        discoveryResult.References.Should().Contain(x => x.To.Contains("DependenSee.csproj"));
    }

    [Fact]
    public void Can_discover_with_wildcard_exclusion()
    {
        var binPath = Path.GetDirectoryName(typeof(ReferenceDiscoveryServiceTests).Assembly.Location);

        {
            var projPath = Path.GetFullPath(Path.Combine(binPath!, "../../../"));

            _output.WriteLine(projPath);

            var referenceDiscovery = new ReferenceDiscoveryService
            {
                SourceFolder = projPath,
                ExcludeProjectsPattern = "**/*.Tests.csproj"
            };

            var discoveryResult = referenceDiscovery.Discover();

            discoveryResult.Projects.Should().HaveCount(0);

            discoveryResult.References.Should().HaveCount(0);
        }

        {
            var projPath = Path.GetFullPath(Path.Combine(binPath!, "../../../../DependenSee"));

            _output.WriteLine(projPath);

            var referenceDiscovery = new ReferenceDiscoveryService
            {
                SourceFolder = projPath,
                ExcludeProjectsPattern = "**/*.Tests.csproj",
                IncludePackages = true
            };

            var discoveryResult = referenceDiscovery.Discover();

            discoveryResult.Projects.Should().HaveCount(1);
            discoveryResult.Projects.Should().Contain(x => x.Name == "DependenSee");

            discoveryResult.References.Should().HaveCountGreaterThanOrEqualTo(2);
            discoveryResult.References.Should().Contain(x => x.To == "PowerArgs");
            discoveryResult.References.Should().Contain(x => x.To == "Microsoft.Extensions.FileSystemGlobbing");
        }
    }
}
