using Microsoft.EntityFrameworkCore;
using WhereTo.Api.Models;
using WhereTo.Api.Security;

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
                Email = "denis@example.org",
                PasswordHash = PasswordService.Hash("Password123!"),
                Role = "Admin"
            },
            new Account
            {
                Name = "Austeja Petrauskaite",
                Email = "austeja@example.org",
                PasswordHash = PasswordService.Hash("Password123!"),
                Role = "User"
            },
            new Account
            {
                Name = "Mantas Jankauskas",
                Email = "mantas@example.org",
                PasswordHash = PasswordService.Hash("Password123!"),
                Role = "User"
            },
            new Account
            {
                Name = "Ieva Kazlauskiene",
                Email = "ieva@example.org",
                PasswordHash = PasswordService.Hash("Password123!"),
                Role = "User"
            },
            new Account
            {
                Name = "Lukas Vaitkus",
                Email = "lukas@example.org",
                PasswordHash = PasswordService.Hash("Password123!"),
                Role = "User"
            }
        };

        foreach (var account in accounts)
        {
            var existing = await db.Accounts
                .SingleOrDefaultAsync(a => a.Email == account.Email, cancellationToken);

            if (existing is null)
            {
                db.Accounts.Add(account);
            }
            else if (string.IsNullOrWhiteSpace(existing.PasswordHash))
            {
                existing.PasswordHash = account.PasswordHash;
                existing.Role = account.Role;
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}