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
                Name = "Demo Account",
                Email = "demo@example.org"
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