using Microsoft.EntityFrameworkCore;
using WhereTo.Api.Models;

namespace WhereTo.Api.Data;

public static class AccountSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        var accounts = new[]
        {
            new Account
            {
                Name = "Denisas Denis",
                Email = "denis@example.org"
            },
            new Account
            {
                Name = "Austeja Petrauskaite",
                Email = "austeja@example.org"
            },
            new Account
            {
                Name = "Mantas Jankauskas",
                Email = "mantas@example.org"
            },
            new Account
            {
                Name = "Ieva Kazlauskiene",
                Email = "ieva@example.org"
            },
            new Account
            {
                Name = "Lukas Vaitkus",
                Email = "lukas@example.org"
            }
        };

        foreach (var account in accounts)
        {
            if (!await db.Accounts.AnyAsync(a => a.Email == account.Email, cancellationToken))
            {
                db.Accounts.Add(account);
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}