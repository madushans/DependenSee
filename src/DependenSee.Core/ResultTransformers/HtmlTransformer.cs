using System.Net;
using System.Reflection;
using System.Text.Json;

namespace DependenSee.Core.ResultTransformers
{
    /// <summary>
    /// Transforms a <see cref="DiscoveryResult"/> into the
    /// DependenSee Visualizer html.
    /// </summary>
    public static class HtmlTransformer
    {
        private const string HtmlTemplateToken = "'{#SOURCE_TOKEN#}'";
        private const string HtmlTitleToken = "{#TITLE_TOKEN#}";
        private const string HtmlTemplateName = "HtmlResultTemplate.html";

        /// <summary>
        /// Transforms a <see cref="DiscoveryResult"/> into the
        /// DependenSee Visualizer html.
        /// </summary>
        public static string Transform(DiscoveryResult result, string title)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName().Name;
            var resourceName = $"{assemblyName}.{HtmlTemplateName}";

            using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new Exception("Could not find embedded HTML Template");
            using var reader = new StreamReader(stream);
            
            var template = reader.ReadToEnd();
            
            var html = template
                .Replace(HtmlTemplateToken, JsonSerializer.Serialize(result))
                .Replace(HtmlTitleToken, WebUtility.HtmlEncode(title));

            return html;
        }
    }
}
