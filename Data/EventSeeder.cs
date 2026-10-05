using Microsoft.EntityFrameworkCore;
using WhereTo.Api.Models;

namespace WhereTo.Api.Data;

public static class EventSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        var organizationIds = await db.Organizations
            .OrderBy(o => o.Id)
            .Select(o => (int?)o.Id)
            .Take(5)
            .ToListAsync(cancellationToken);

        int? OrganizationAt(int index) => index < organizationIds.Count ? organizationIds[index] : null;

        // Datos skaiciuojamos nuo siandien
        var today = DateTime.Today;

        var events = new[]
        {
            new Event
            {
                OrganizationId = OrganizationAt(0),
                Title = "Night Market Sessions",
                Category = "Music",
                Description = "Live music, street food and local makers.",
                Venue = "Užupis Art Incubator",
                City = "Vilnius",
                EventDate = today.AddDays(14).AddHours(19),
                StartingPrice = 28,
                TotalTickets = 300,
                AvailableTickets = 300
            },
            new Event
            {
                OrganizationId = OrganizationAt(1),
                Title = "Designing Tomorrow",
                Category = "Talks",
                Description = "Talks about design, technology and cities.",
                Venue = "Kaunas Town Hall",
                City = "Kaunas",
                EventDate = today.AddDays(21).AddHours(10),
                StartingPrice = 45,
                TotalTickets = 180,
                AvailableTickets = 180
            },
            new Event
            {
                OrganizationId = OrganizationAt(2),
                Title = "Sunday Clay Club",
                Category = "Workshop",
                Description = "Beginner-friendly ceramics workshop.",
                Venue = "Old Town Ceramics Studio",
                City = "Vilnius",
                EventDate = today.AddDays(30).AddHours(14),
                StartingPrice = 32,
                TotalTickets = 24,
                AvailableTickets = 24
            },
            new Event
            {
                OrganizationId = OrganizationAt(3),
                Title = "Riverbank Film Night",
                Category = "Film",
                Description = "Independent films under the open sky.",
                Venue = "Nemunas Riverside",
                City = "Kaunas",
                EventDate = today.AddDays(37).AddHours(20),
                StartingPrice = 18,
                TotalTickets = 120,
                AvailableTickets = 120
            },
            new Event
            {
                OrganizationId = OrganizationAt(4),
                Title = "Morning Makers Fair",
                Category = "Market",
                Description = "Handmade goods, coffee and conversations with local makers.",
                Venue = "Halles Market",
                City = "Vilnius",
                EventDate = today.AddDays(45).AddHours(11),
                StartingPrice = 8,
                TotalTickets = 200,
                AvailableTickets = 200
            }
        };

        foreach (var eventItem in events)
        {
            if (!await db.Events.AnyAsync(e => e.Title == eventItem.Title, cancellationToken))
            {
                db.Events.Add(eventItem);
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
