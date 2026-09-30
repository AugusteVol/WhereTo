using Microsoft.EntityFrameworkCore;
using WhereTo.Api.Models;

namespace WhereTo.Api.Data;

public static class TicketSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        var ticketCount = await db.Tickets.CountAsync(cancellationToken);
        if (ticketCount >= 3)
        {
            return;
        }

        var accounts = await db.Accounts
            .OrderBy(a => a.Id)
            .Take(3)
            .ToListAsync(cancellationToken);

        var events = await db.Events
            .OrderBy(e => e.Id)
            .Take(3)
            .ToListAsync(cancellationToken);

        var ticketsToAdd = 3 - ticketCount;
        for (var index = 0; index < ticketsToAdd && index < accounts.Count && index < events.Count; index++)
        {
            var eventItem = events[index];
            db.Tickets.Add(new Ticket
            {
                EventId = eventItem.Id,
                AccountId = accounts[index].Id,
                Price = eventItem.StartingPrice,
                PurchasedAt = DateTime.UtcNow.AddDays(-(index + 1))
            });
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}