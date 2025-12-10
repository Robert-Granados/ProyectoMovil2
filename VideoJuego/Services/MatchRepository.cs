using VideoJuego.Models;

namespace VideoJuego.Services;

public class MatchRepository : IMatchRepository
{
    private readonly IDatabaseService _database;

    public MatchRepository(IDatabaseService database)
    {
        _database = database;
    }

    public async Task AddAsync(MatchRecord record)
    {
        await _database.InitializeAsync();
        await using var connection = _database.GetConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Matches(Date, Player1Id, Player2Id, WinnerPlayerId, WasDraw, Player1Name, Player2Name, Player1Race, Player2Race, Player1Variant, Player2Variant)
                                VALUES ($date, $p1, $p2, $winner, $draw, $p1name, $p2name, $p1race, $p2race, $p1var, $p2var);";
        command.Parameters.AddWithValue("$date", record.Date);
        command.Parameters.AddWithValue("$p1", record.Player1Id);
        command.Parameters.AddWithValue("$p2", record.Player2Id);
        command.Parameters.AddWithValue("$winner", (object?)record.WinnerPlayerId ?? DBNull.Value);
        command.Parameters.AddWithValue("$draw", record.WasDraw ? 1 : 0);
        command.Parameters.AddWithValue("$p1name", record.Player1Name);
        command.Parameters.AddWithValue("$p2name", record.Player2Name);
        command.Parameters.AddWithValue("$p1race", (int)record.Player1Race);
        command.Parameters.AddWithValue("$p2race", (int)record.Player2Race);
        command.Parameters.AddWithValue("$p1var", record.Player1Variant);
        command.Parameters.AddWithValue("$p2var", record.Player2Variant);

        await command.ExecuteNonQueryAsync();
    }
}
