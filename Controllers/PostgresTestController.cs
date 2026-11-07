using System.Text;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Storefront.Controllers;

public class PostgresTestController : Controller
{
    private readonly IConfiguration _configuration;

    public PostgresTestController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IActionResult> Index(string? connectionString = null, string? query = null)
    {
        var resolvedConnectionString = connectionString
            ?? _configuration.GetConnectionString("PostgresConnection")
            ?? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(resolvedConnectionString))
        {
            return Content(
                "No PostgreSQL connection string found. Set ConnectionStrings:PostgresConnection or POSTGRES_CONNECTION_STRING.");
        }

        var resolvedQuery = query ?? "SELECT id, name FROM users;";

        try
        {
            await using var dataSource = NpgsqlDataSource.Create(resolvedConnectionString);
            await using var command = dataSource.CreateCommand(resolvedQuery);
            await using var reader = await command.ExecuteReaderAsync();

            var output = new StringBuilder();
            output.AppendLine("Connected successfully.");
            output.AppendLine("--- Query Results ---");

            var hasRows = false;
            while (await reader.ReadAsync())
            {
                hasRows = true;

                var values = new List<string>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    values.Add(reader.IsDBNull(i) ? "NULL" : reader.GetValue(i)?.ToString() ?? "NULL");
                }

                output.AppendLine(string.Join(" | ", values));
            }

            if (!hasRows)
            {
                output.AppendLine("No rows returned.");
            }

            return Content(output.ToString());
        }
        catch (Exception ex)
        {
            return Content($"Connection failed: {ex.Message}");
        }
    }
}
