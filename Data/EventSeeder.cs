using Microsoft.EntityFrameworkCore;
using WhereTo.Api.Models;

namespace WhereTo.Api.Data;

public static class EventSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Events.AnyAsync(cancellationToken))
        {
            return;
        }

        var organizationIds = await db.Organizations
            .OrderBy(o => o.Id)
            .Select(o => (int?)o.Id)
            .Take(3)
            .ToListAsync(cancellationToken);

        int? OrganizationAt(int index) => index < organizationIds.Count ? organizationIds[index] : null;

        // Datos skaičiuojamos nuo šiandien, kad /api/events (rodo tik artėjančius) visada turėtų duomenų
        var today = DateTime.Today;

        db.Events.AddRange(
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
            });

        await db.SaveChangesAsync(cancellationToken);
    }
}
