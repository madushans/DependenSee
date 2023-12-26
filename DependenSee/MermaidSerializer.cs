namespace DependenSee;
public static class MermaidSerializer
{
	const string ProjectBackgroundColor = "#00cc22";
	const string PackageBackgroundColor = "#22aaee";

	public static string ToString(DiscoveryResult discoveryResult)
	{
		using var stringWriter = new StringWriter();
		using var writer = new IndentedTextWriter(stringWriter);

		writer.WriteLine("graph LR");

		writer.Indent++;

		foreach (var project in discoveryResult.Projects)
		{
			writer.WriteLine($"{project.Id}[\"{project.Name}\"]:::project");
		}

		if (discoveryResult.Packages.Count > 0)
		{
			writer.WriteLine($"subgraph Packages");
			foreach (var package in discoveryResult.Packages)
			{
				writer.WriteLine($"{package.Id}[\"{package.Name}\"]:::package");
			}
			writer.WriteLine("end");
		}

		foreach (var reference in discoveryResult.References)
		{
			writer.WriteLine($"{reference.From} --> {reference.To}");
		}

		writer.WriteLine($"classDef project fill:{ProjectBackgroundColor};");
		writer.WriteLine($"classDef package fill:{PackageBackgroundColor};");

		writer.Indent--;

		writer.Flush();

		return stringWriter.ToString();
	}
}
