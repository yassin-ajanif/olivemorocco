namespace OliveMorocco.Web.Models;

/// <summary>Server-rendered list page shell for dashboard modules.</summary>
public sealed class ModuleListPageViewModel
{
    public required string Section { get; init; }
    public required string ModuleKey { get; init; }
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public required IReadOnlyList<string> Headers { get; init; }
    public required string GridColumns { get; init; }
    public int TotalCount { get; init; }
}
