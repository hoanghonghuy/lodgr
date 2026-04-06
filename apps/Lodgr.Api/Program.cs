using Lodgr.Api.Data;
using Lodgr.Api.Features.Buildings;
using Lodgr.Api.Features.Contracts;
using Lodgr.Api.Features.Rooms;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddProblemDetails();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? $"Host={builder.Configuration["POSTGRES_HOST"] ?? "localhost"};"
    + $"Port={builder.Configuration["POSTGRES_PORT"] ?? "5432"};"
    + $"Database={builder.Configuration["POSTGRES_DB"] ?? "lodgr"};"
    + $"Username={builder.Configuration["POSTGRES_USER"] ?? "lodgr_user"};"
    + $"Password={builder.Configuration["POSTGRES_PASSWORD"] ?? "change_me"}";

builder.Services.AddDbContext<LodgrDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IBuildingRepository, BuildingRepository>();
builder.Services.AddScoped<IBuildingService, BuildingService>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
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

app.MapControllers();

app.Run();
