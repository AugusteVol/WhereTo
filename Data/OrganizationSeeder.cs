using Microsoft.EntityFrameworkCore;
using WhereTo.Api.Models;

namespace WhereTo.Api.Data;

public static class OrganizationSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        if (!await db.Database.CanConnectAsync(cancellationToken))
        {
            return;
        }

        if (await db.Organizations.AnyAsync(cancellationToken))
        {
            return;
        }

        db.Organizations.AddRange(
            new Organization
            {
                Name = "Vilnius Arts Collective",
                Description = "A community organization hosting arts and culture events in Vilnius.",
                Email = "vilnius-arts@example.org",
                Website = "https://example.org/vilnius-arts"
            },
            new Organization
            {
                Name = "Kaunas Community Stage",
                Description = "Local performances, workshops, and gatherings in Kaunas.",
                Email = "kaunas-stage@example.org",
                Website = "https://example.org/kaunas-stage"
            },
            new Organization
            {
                Name = "Baltic Culture Network",
                Description = "A network connecting independent cultural event organizers.",
                Email = "baltic-culture@example.org",
                Website = "https://example.org/baltic-culture"
            },
            new Organization
            {
                Name = "Meno Klubas",
                Description = "Klubas meno nariams.",
                Email = "menas@example.org",
                Website = "https://example.org/meno-klubas"
            },
            new Organization
            {
                Name = "Popieriaus lankstytojai",
                Description = "megsti popieriu? mes irgi.",
                Email = "popierius@example.org",
                Website = "https://example.org/poperiauslankstytojai"
            });

        await db.SaveChangesAsync(cancellationToken);
    }
}