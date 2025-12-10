using Microsoft.Data.Sqlite;
using VideoJuego.Models;

namespace VideoJuego.Services;

public class PlayerRepository : IPlayerRepository
{
    private readonly IDatabaseService _database;

    public PlayerRepository(IDatabaseService database)
    {
        _database = database;
    }

    public async Task<PlayerProfile> GetOrCreateAsync(string name)
    {
        await _database.InitializeAsync();
        await using var connection = _database.GetConnection();
        await connection.OpenAsync();

        await using (var select = connection.CreateCommand())
        {
            select.CommandText = "SELECT Id, Name, Wins, Losses, Draws FROM Players WHERE Name = $name;";
            select.Parameters.AddWithValue("$name", name);
            await using var reader = await select.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return ReadPlayer(reader);
            }
        }

        await using (var insert = connection.CreateCommand())
        {
            insert.CommandText = "INSERT INTO Players (Name, Wins, Losses, Draws) VALUES ($name, 0, 0, 0); SELECT last_insert_rowid();";
            insert.Parameters.AddWithValue("$name", name);
            var newId = (long)(await insert.ExecuteScalarAsync() ?? 0);
            return new PlayerProfile { Id = (int)newId, Name = name, Wins = 0, Losses = 0, Draws = 0 };
        }
    }

    public async Task<List<PlayerProfile>> GetAllAsync()
    {
        await _database.InitializeAsync();
        await using var connection = _database.GetConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Wins, Losses, Draws FROM Players ORDER BY Wins DESC, Name ASC;";
        await using var reader = await command.ExecuteReaderAsync();

        var results = new List<PlayerProfile>();
        while (await reader.ReadAsync())
        {
            results.Add(ReadPlayer(reader));
        }

        return results;
    }

    public async Task UpdateStatsAsync(int playerId, bool won, bool draw)
    {
        await _database.InitializeAsync();
        await using var connection = _database.GetConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = won
            ? "UPDATE Players SET Wins = Wins + 1 WHERE Id = $id"
            : draw
                ? "UPDATE Players SET Draws = Draws + 1 WHERE Id = $id"
                : "UPDATE Players SET Losses = Losses + 1 WHERE Id = $id";

        command.Parameters.AddWithValue("$id", playerId);
        await command.ExecuteNonQueryAsync();
    }

    private static PlayerProfile ReadPlayer(SqliteDataReader reader)
    {
        return new PlayerProfile
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Wins = reader.GetInt32(2),
            Losses = reader.GetInt32(3),
            Draws = reader.GetInt32(4)
        };
    }
}
