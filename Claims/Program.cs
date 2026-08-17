using Claims.Auditing;
using Claims.Data;
using Claims.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Text.Json.Serialization;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

var builder = WebApplication.CreateBuilder(args);

var isTesting = builder.Environment.IsEnvironment("Testing");

MsSqlContainer? sqlContainer = null;
MongoDbContainer? mongoContainer = null;

if (!isTesting)
{
    sqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPortBinding(14330, 1433)
        .WithCleanUp(true)
        .Build();

    mongoContainer = new MongoDbBuilder("mongo:latest")
        .WithPortBinding(27018, 27017)
        .WithCleanUp(true)
        .Build();

    await sqlContainer.StartAsync();
    await mongoContainer.StartAsync();

    Console.WriteLine($"SQL Server: {sqlContainer.GetConnectionString()}");
    Console.WriteLine($"MongoDB:    {mongoContainer.GetConnectionString()}");
}

builder.Services
    .AddClaimsData()
    .AddClaimsServices()
    .AddClaimsAuditing();

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

if (!isTesting)
{
    builder.Services.AddDbContext<AuditContext>(options =>
        options.UseSqlServer(sqlContainer!.GetConnectionString()));

    builder.Services.AddDbContext<ClaimsContext>(options =>
    {
        var client = new MongoClient(mongoContainer!.GetConnectionString());
        var database = client.GetDatabase(builder.Configuration["MongoDb:DatabaseName"]);
        options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
    });
}
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

if (!isTesting)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}


app.UseExceptionHandler(errApp =>
{
    errApp.Run(async context =>
    {
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (feature?.Error is FluentValidation.ValidationException validationEx)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                errors = validationEx.Errors.Select(e => e.ErrorMessage)
            });
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    });
});

app.Run();

public partial class Program { }
