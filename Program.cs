using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using WhereTo.Api.Data;
using WhereTo.Api.Security;

var builder = WebApplication.CreateBuilder(args);

// DB
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Nerasta ConnectionStrings:Default konfigūracija.");

// .db failas visada aplanke
var sqlite = new SqliteConnectionStringBuilder(connectionString);
sqlite.DataSource = Path.Combine(builder.Environment.ContentRootPath, sqlite.DataSource);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(sqlite.ToString()));

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Nerasta Jwt:Key konfigūracija.");
builder.Services.AddSingleton<JwtTokenService>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });
builder.Services.AddAuthorization();

// api
builder.Services.AddControllers();
builder.Services.AddProblemDetails(); // vienodas klaidų formatas (RFC 7807)

// Reactas
//var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
//    ?? ["http://localhost:5173"];
//
//builder.Services.AddCors(options =>
//    options.AddPolicy("Client", policy =>
//        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

// Swaggeris
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WhereTo API",
        Version = "v1",
        Description = "Renginių radimo ir bilietų pirkimo platformos API."
    });

    // itrauki /// komentarus is kodo i swagger web
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync(); // sukuria whereto.db ir lenteles, jei jų dar nėra
    await EnsureAuthenticationColumnsAsync(sqlite.ToString());
    await EnsureRefreshTokensTableAsync(sqlite.ToString());
    await OrganizationSeeder.SeedAsync(db);
    await EventSeeder.SeedAsync(db);
    await AccountSeeder.SeedAsync(db);
    await TicketSeeder.SeedAsync(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(); // 500 -> ProblemDetails be stack trace
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // /swagger/v1/swagger.json
    app.UseSwaggerUI();     // /swagger
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.UseCors("Client");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

static async Task EnsureAuthenticationColumnsAsync(string connectionString)
{
    await using var connection = new SqliteConnection(connectionString);
    await connection.OpenAsync();
    await AddColumnIfMissingAsync(connection, "password_hash", "TEXT NOT NULL DEFAULT ''");
    await AddColumnIfMissingAsync(connection, "role", "TEXT NOT NULL DEFAULT 'User'");
}

static async Task EnsureRefreshTokensTableAsync(string connectionString)
{
    await using var connection = new SqliteConnection(connectionString);
    await connection.OpenAsync();
    await using var command = connection.CreateCommand();
    command.CommandText = """
        CREATE TABLE IF NOT EXISTS refresh_tokens (
            id INTEGER NOT NULL CONSTRAINT pk_refresh_tokens PRIMARY KEY AUTOINCREMENT,
            account_id INTEGER NOT NULL,
            token_hash TEXT NOT NULL,
            expires_at TEXT NOT NULL,
            created_at TEXT NOT NULL,
            revoked_at TEXT NULL,
            CONSTRAINT fk_refresh_tokens_accounts_account_id
                FOREIGN KEY (account_id) REFERENCES accounts (id) ON DELETE CASCADE
        );
        CREATE UNIQUE INDEX IF NOT EXISTS ix_refresh_tokens_token_hash
            ON refresh_tokens (token_hash);
        """;
    await command.ExecuteNonQueryAsync();
}

static async Task AddColumnIfMissingAsync(SqliteConnection connection, string columnName, string definition)
{
    await using var check = connection.CreateCommand();
    check.CommandText = "SELECT COUNT(*) FROM pragma_table_info('accounts') WHERE name = $name";
    check.Parameters.AddWithValue("$name", columnName);
    var exists = Convert.ToInt32(await check.ExecuteScalarAsync()) > 0;
    if (exists)
    {
        return;
    }

    await using var alter = connection.CreateCommand();
    alter.CommandText = $"ALTER TABLE accounts ADD COLUMN {columnName} {definition}";
    await alter.ExecuteNonQueryAsync();
}
