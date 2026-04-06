using Lodgr.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? $"Host={builder.Configuration["POSTGRES_HOST"] ?? "localhost"};"
    + $"Port={builder.Configuration["POSTGRES_PORT"] ?? "5432"};"
    + $"Database={builder.Configuration["POSTGRES_DB"] ?? "lodgr"};"
    + $"Username={builder.Configuration["POSTGRES_USER"] ?? "lodgr_user"};"
    + $"Password={builder.Configuration["POSTGRES_PASSWORD"] ?? "change_me"}";

builder.Services.AddDbContext<LodgrDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "lodgr-api" }))
    .WithName("HealthCheck");

app.MapGet("/health/db", async (LodgrDbContext db, CancellationToken cancellationToken) =>
{
    var canConnect = await db.Database.CanConnectAsync(cancellationToken);
    return canConnect
        ? Results.Ok(new { status = "ok", database = "reachable" })
        : Results.Problem(
            detail: "Cannot connect to PostgreSQL.",
            statusCode: StatusCodes.Status503ServiceUnavailable,
            title: "Database unavailable");
})
.WithName("DatabaseHealthCheck");

app.Run();
