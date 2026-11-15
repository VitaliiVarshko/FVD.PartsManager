
using FVD.PartsManager.Web.Components;
using FVD.PartsManager.Web.Data;
using FVD.PartsManager.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Blazor-Komponenten registrieren
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// PostgreSQL-Datenbank konfigurieren
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Die Datenbankverbindung 'DefaultConnection' fehlt.");

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Anwendungsdienste registrieren
builder.Services.AddScoped<ArtikelService>();
builder.Services.AddScoped<LagerbewegungService>();

var app = builder.Build();


// Datenbankmigrationen nur auf ausdrücklichen Befehl ausführen.
if (args.Contains("--migrate", StringComparer.OrdinalIgnoreCase))
{
    await using var scope = app.Services.CreateAsyncScope();

    var dbFactory = scope.ServiceProvider
        .GetRequiredService<IDbContextFactory<AppDbContext>>();

    await using var db = await dbFactory.CreateDbContextAsync();

    await db.Database.MigrateAsync();

    Console.WriteLine("Datenbankmigrationen erfolgreich angewendet.");
    return;
}


// Demo-Daten nur nach ausdrücklicher Freigabe importieren.
if (args.Contains("--seed-demo", StringComparer.OrdinalIgnoreCase))
{
    if (!builder.Configuration.GetValue<bool>("DemoData:AllowSeeding"))
    {
        throw new InvalidOperationException(
            "Demo-Seeding ist nicht freigegeben. " +
            "DemoData:AllowSeeding muss auf true gesetzt sein.");
    }

    await DemoDataSeeder.SeedAsync(
        app.Services.GetRequiredService<
            IDbContextFactory<AppDbContext>>());

    return;
}

// HTTP-Pipeline konfigurieren
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
