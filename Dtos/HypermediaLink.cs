namespace WhereTo.Api.Dtos;

/// <summary>Nuoroda į susijusį API veiksmą.</summary>
public class HypermediaLink
{
    public string Rel { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string Method { get; set; } = "GET";
}