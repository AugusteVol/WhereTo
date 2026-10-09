using Microsoft.EntityFrameworkCore;
using WhereTo.Api.Models;

namespace WhereTo.Api.Data;

public static class TicketSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        var accounts = await db.Accounts
            .OrderBy(a => a.Id)
            .ToListAsync(cancellationToken);

        var events = await db.Events
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);

        if (accounts.Count == 0 || events.Count == 0)
        {
            return;
        }

        const int eventIdWithOneTicket = 4;
        var eventFourTickets = await db.Tickets
            .Where(ticket => ticket.EventId == eventIdWithOneTicket)
            .OrderBy(ticket => ticket.Id)
            .ToListAsync(cancellationToken);

        if (eventFourTickets.Count > 1)
        {
            db.Tickets.RemoveRange(eventFourTickets.Skip(1));
        }

        var eventFourAlreadyHasTicket = eventFourTickets.Count > 0;

        for (var accountIndex = 0; accountIndex < accounts.Count; accountIndex++)
        {
            var account = accounts[accountIndex];
            var existingTicketCount = await db.Tickets
                .CountAsync(ticket => ticket.AccountId == account.Id, cancellationToken);

            for (var ticketIndex = existingTicketCount; ticketIndex < 2; ticketIndex++)
            {
                var eventItem = events[(accountIndex + ticketIndex) % events.Count];
                if (eventItem.Id == eventIdWithOneTicket && eventFourAlreadyHasTicket)
                {
                    eventItem = events.First(eventItem => eventItem.Id != eventIdWithOneTicket);
                }

                if (eventItem.Id == eventIdWithOneTicket)
                {
                    eventFourAlreadyHasTicket = true;
                }

                db.Tickets.Add(new Ticket
                {
                    EventId = eventItem.Id,
                    AccountId = account.Id,
                    Price = eventItem.StartingPrice,
                    PurchasedAt = DateTime.UtcNow.AddDays(-(accountIndex + ticketIndex + 1))
                });
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}