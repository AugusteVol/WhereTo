using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WhereTo.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// ---------- Duomenų bazė (SQLite: vienas failas, jokio serverio / XAMPP nereikia) ----------
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Nerasta ConnectionStrings:Default konfigūracija.");

// whereto.db visada guli projekto aplanke, nesvarbu iš kur paleista programa
var sqlite = new SqliteConnectionStringBuilder(connectionString);
sqlite.DataSource = Path.Combine(builder.Environment.ContentRootPath, sqlite.DataSource);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(sqlite.ToString()));

// ---------- API ----------
builder.Services.AddControllers();
builder.Services.AddProblemDetails(); // vienodas klaidų formatas (RFC 7807)

// ---------- CORS (React klientas) ----------
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];

builder.Services.AddCors(options =>
    options.AddPolicy("Client", policy =>
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

// ---------- Swagger / OpenAPI ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WhereTo API",
        Version = "v1",
        Description = "Renginių radimo ir bilietų pirkimo platformos API."
    });

    // Įtraukiame /// komentarus iš kodo į Swagger dokumentaciją
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
    await OrganizationSeeder.SeedAsync(db);
    await EventSeeder.SeedAsync(db);
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
app.MapControllers();

app.Run();
