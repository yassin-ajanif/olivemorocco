using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;

namespace OliveMorocco.Web.Routing;

/// <summary>
/// Resolves views under <c>Views/{Vente|Achat|Operationnel|Stockage}/{Controller}/</c>
/// for controllers in matching <c>Controllers.*</c> subfolders (see docs/ARCHITECTURE.md).
/// Sections may nest feature folders (e.g. <c>Controllers.Stockage.Intrants</c>) —
/// only the first segment identifies the section.
/// </summary>
public sealed class SectionViewLocationExpander : IViewLocationExpander
{
    private const string SectionKey = "section";

    public void PopulateValues(ViewLocationExpanderContext context)
    {
        if (context.ActionContext.ActionDescriptor is not ControllerActionDescriptor descriptor)
            return;

        var section = GetSection(descriptor.ControllerTypeInfo.Namespace);
        if (section is not null)
            context.Values[SectionKey] = section;
    }

    public IEnumerable<string> ExpandViewLocations(
        ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations)
    {
        if (!context.Values.TryGetValue(SectionKey, out var section))
            return viewLocations;

        var sectionLocations = new[]
        {
            $"/Views/{section}/{{1}}/{{0}}.cshtml",
        };

        return sectionLocations.Concat(viewLocations);
    }

    private static string? GetSection(string? controllerNamespace)
    {
        if (string.IsNullOrEmpty(controllerNamespace))
            return null;

        const string prefix = "OliveMorocco.Web.Controllers.";
        if (!controllerNamespace.StartsWith(prefix, StringComparison.Ordinal))
            return null;

        var relative = controllerNamespace[prefix.Length..];
        if (relative.Length == 0)
            return null;

        // Sections may nest feature folders, e.g. "Stockage.Intrants" — take the section only.
        var sectionEnd = relative.IndexOf('.', StringComparison.Ordinal);
        var section = sectionEnd < 0 ? relative : relative[..sectionEnd];

        return section.Length == 0 ? null : section;
    }
}
