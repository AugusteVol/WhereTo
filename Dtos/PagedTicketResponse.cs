namespace WhereTo.Api.Dtos;

/// <summary>Puslapiuotas bilietų sąrašas.</summary>
public class PagedTicketResponse
{
    public IEnumerable<TicketResponse> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<HypermediaLink> Links { get; set; } = [];
}