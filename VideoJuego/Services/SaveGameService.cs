using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using VideoJuego.Models;

namespace VideoJuego.Services;

public class SaveGameService : ISaveGameService
{
    private readonly IDatabaseService _database;

    public SaveGameService(IDatabaseService database)
    {
        _database = database;
    }

    public async Task SaveGameAsync(SavedGame state)
    {
        await _database.InitializeAsync();
        await using var connection = _database.GetConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO SavedGames(
                                    Id, Player1Name, Player2Name, Player1Race, Player2Race,
                                    Player1HumanWeapon, Player2HumanWeapon, Player1ElfElement, Player2ElfElement,
                                    Player1OrcWeapon, Player2OrcWeapon, Player1BeastAttack, Player2BeastAttack,
                                    Player1CurrentHp, Player2CurrentHp, Player1MaxHp, Player2MaxHp,
                                    Distance, TurnNumber, ActiveIndex,
                                    Player1Bleeding, Player2Bleeding, Player1BleedingTurnsLeft, Player2BleedingTurnsLeft,
                                    Player1PendingHeal, Player2PendingHeal,
                                    CurrentTurnPlayerName, LastSavedAt, LogJson)
                                VALUES (
                                    $id, $p1name, $p2name, $p1race, $p2race,
                                    $p1human, $p2human, $p1elf, $p2elf,
                                    $p1orc, $p2orc, $p1beast, $p2beast,
                                    $p1hp, $p2hp, $p1max, $p2max,
                                    $distance, $turn, $active,
                                    $p1bleed, $p2bleed, $p1bleedTurns, $p2bleedTurns,
                                    $p1heal, $p2heal,
                                    $currentTurn, $savedAt, $log)
                                ON CONFLICT(Id) DO UPDATE SET
                                    Player1Name = excluded.Player1Name,
                                    Player2Name = excluded.Player2Name,
                                    Player1Race = excluded.Player1Race,
                                    Player2Race = excluded.Player2Race,
                                    Player1HumanWeapon = excluded.Player1HumanWeapon,
                                    Player2HumanWeapon = excluded.Player2HumanWeapon,
                                    Player1ElfElement = excluded.Player1ElfElement,
                                    Player2ElfElement = excluded.Player2ElfElement,
                                    Player1OrcWeapon = excluded.Player1OrcWeapon,
                                    Player2OrcWeapon = excluded.Player2OrcWeapon,
                                    Player1BeastAttack = excluded.Player1BeastAttack,
                                    Player2BeastAttack = excluded.Player2BeastAttack,
                                    Player1CurrentHp = excluded.Player1CurrentHp,
                                    Player2CurrentHp = excluded.Player2CurrentHp,
                                    Player1MaxHp = excluded.Player1MaxHp,
                                    Player2MaxHp = excluded.Player2MaxHp,
                                    Distance = excluded.Distance,
                                    TurnNumber = excluded.TurnNumber,
                                    ActiveIndex = excluded.ActiveIndex,
                                    Player1Bleeding = excluded.Player1Bleeding,
                                    Player2Bleeding = excluded.Player2Bleeding,
                                    Player1BleedingTurnsLeft = excluded.Player1BleedingTurnsLeft,
                                    Player2BleedingTurnsLeft = excluded.Player2BleedingTurnsLeft,
                                    Player1PendingHeal = excluded.Player1PendingHeal,
                                    Player2PendingHeal = excluded.Player2PendingHeal,
                                    CurrentTurnPlayerName = excluded.CurrentTurnPlayerName,
                                    LastSavedAt = excluded.LastSavedAt,
                                    LogJson = excluded.LogJson;";

        command.Parameters.AddWithValue("$id", state.Id);
        command.Parameters.AddWithValue("$p1name", state.Player1Name);
        command.Parameters.AddWithValue("$p2name", state.Player2Name);
        command.Parameters.AddWithValue("$p1race", (int)state.Player1Race);
        command.Parameters.AddWithValue("$p2race", (int)state.Player2Race);
        command.Parameters.AddWithValue("$p1human", (object?)state.Player1HumanWeapon ?? DBNull.Value);
        command.Parameters.AddWithValue("$p2human", (object?)state.Player2HumanWeapon ?? DBNull.Value);
        command.Parameters.AddWithValue("$p1elf", (object?)state.Player1ElfElement ?? DBNull.Value);
        command.Parameters.AddWithValue("$p2elf", (object?)state.Player2ElfElement ?? DBNull.Value);
        command.Parameters.AddWithValue("$p1orc", (object?)state.Player1OrcWeapon ?? DBNull.Value);
        command.Parameters.AddWithValue("$p2orc", (object?)state.Player2OrcWeapon ?? DBNull.Value);
        command.Parameters.AddWithValue("$p1beast", (object?)state.Player1BeastAttack ?? DBNull.Value);
        command.Parameters.AddWithValue("$p2beast", (object?)state.Player2BeastAttack ?? DBNull.Value);
        command.Parameters.AddWithValue("$p1hp", state.Player1CurrentHp);
        command.Parameters.AddWithValue("$p2hp", state.Player2CurrentHp);
        command.Parameters.AddWithValue("$p1max", state.Player1MaxHp);
        command.Parameters.AddWithValue("$p2max", state.Player2MaxHp);
        command.Parameters.AddWithValue("$distance", state.Distance);
        command.Parameters.AddWithValue("$turn", state.TurnNumber);
        command.Parameters.AddWithValue("$active", state.ActiveIndex);
        command.Parameters.AddWithValue("$p1bleed", state.Player1Bleeding ? 1 : 0);
        command.Parameters.AddWithValue("$p2bleed", state.Player2Bleeding ? 1 : 0);
        command.Parameters.AddWithValue("$p1bleedTurns", state.Player1BleedingTurnsLeft);
        command.Parameters.AddWithValue("$p2bleedTurns", state.Player2BleedingTurnsLeft);
        command.Parameters.AddWithValue("$p1heal", state.Player1PendingHeal);
        command.Parameters.AddWithValue("$p2heal", state.Player2PendingHeal);
        command.Parameters.AddWithValue("$currentTurn", state.CurrentTurnPlayerName);
        command.Parameters.AddWithValue("$savedAt", state.LastSavedAt);
        command.Parameters.AddWithValue("$log", state.LogJson);

        await command.ExecuteNonQueryAsync();
    }

    public Task<SavedGame?> LoadGameAsync()
    {
        return LoadMostRecentAsync();
    }

    public Task<SavedGame?> LoadGameAsync(int id)
    {
        return LoadByIdAsync(id);
    }

    public async Task<List<SavedGame>> GetAllAsync()
    {
        await _database.InitializeAsync();
        await using var connection = _database.GetConnection();
        await connection.OpenAsync();

        var results = new List<SavedGame>();

        await using var command = connection.CreateCommand();
        command.CommandText = @"SELECT Id, Player1Name, Player2Name, Player1Race, Player2Race,
                                       Player1HumanWeapon, Player2HumanWeapon, Player1ElfElement, Player2ElfElement,
                                       Player1OrcWeapon, Player2OrcWeapon, Player1BeastAttack, Player2BeastAttack,
                                       Player1CurrentHp, Player2CurrentHp, Player1MaxHp, Player2MaxHp,
                                       Distance, TurnNumber, ActiveIndex,
                                       Player1Bleeding, Player2Bleeding, Player1BleedingTurnsLeft, Player2BleedingTurnsLeft,
                                       Player1PendingHeal, Player2PendingHeal,
                                       CurrentTurnPlayerName, LastSavedAt, LogJson
                                FROM SavedGames
                                ORDER BY datetime(LastSavedAt) DESC;";

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(MapSavedGame(reader));
        }

        return results;
    }

    private async Task<SavedGame?> LoadMostRecentAsync()
    {
        await _database.InitializeAsync();
        await using var connection = _database.GetConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = @"SELECT Id, Player1Name, Player2Name, Player1Race, Player2Race,
                                       Player1HumanWeapon, Player2HumanWeapon, Player1ElfElement, Player2ElfElement,
                                       Player1OrcWeapon, Player2OrcWeapon, Player1BeastAttack, Player2BeastAttack,
                                       Player1CurrentHp, Player2CurrentHp, Player1MaxHp, Player2MaxHp,
                                       Distance, TurnNumber, ActiveIndex,
                                       Player1Bleeding, Player2Bleeding, Player1BleedingTurnsLeft, Player2BleedingTurnsLeft,
                                       Player1PendingHeal, Player2PendingHeal,
                                       CurrentTurnPlayerName, LastSavedAt, LogJson
                                FROM SavedGames
                                ORDER BY datetime(LastSavedAt) DESC
                                LIMIT 1;";

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return MapSavedGame(reader);
    }

    private async Task<SavedGame?> LoadByIdAsync(int id)
    {
        await _database.InitializeAsync();
        await using var connection = _database.GetConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = @"SELECT Id, Player1Name, Player2Name, Player1Race, Player2Race,
                                       Player1HumanWeapon, Player2HumanWeapon, Player1ElfElement, Player2ElfElement,
                                       Player1OrcWeapon, Player2OrcWeapon, Player1BeastAttack, Player2BeastAttack,
                                       Player1CurrentHp, Player2CurrentHp, Player1MaxHp, Player2MaxHp,
                                       Distance, TurnNumber, ActiveIndex,
                                       Player1Bleeding, Player2Bleeding, Player1BleedingTurnsLeft, Player2BleedingTurnsLeft,
                                       Player1PendingHeal, Player2PendingHeal,
                                       CurrentTurnPlayerName, LastSavedAt, LogJson
                                FROM SavedGames
                                WHERE Id = $id
                                LIMIT 1;";
        command.Parameters.AddWithValue("$id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return MapSavedGame(reader);
    }

    public async Task<bool> HasSavedGameAsync()
    {
        await _database.InitializeAsync();
        await using var connection = _database.GetConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM SavedGames;";
        var count = (long?)await command.ExecuteScalarAsync();
        return count.GetValueOrDefault() > 0;
    }

    public async Task DeleteSavedGameAsync()
    {
        await _database.InitializeAsync();
        await using var connection = _database.GetConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM SavedGames;";
        await command.ExecuteNonQueryAsync();
    }

    private static SavedGame MapSavedGame(SqliteDataReader reader)
    {
        var savedAtRaw = reader.IsDBNull(27) ? string.Empty : reader.GetString(27);
        _ = DateTime.TryParse(savedAtRaw, out var savedAt);

        var saved = new SavedGame
        {
            Id = reader.GetInt32(0),
            Player1Name = reader.GetString(1),
            Player2Name = reader.GetString(2),
            Player1Race = (Race)reader.GetInt32(3),
            Player2Race = (Race)reader.GetInt32(4),
            Player1HumanWeapon = reader.IsDBNull(5) ? null : (HumanWeapon?)reader.GetInt32(5),
            Player2HumanWeapon = reader.IsDBNull(6) ? null : (HumanWeapon?)reader.GetInt32(6),
            Player1ElfElement = reader.IsDBNull(7) ? null : (ElfElement?)reader.GetInt32(7),
            Player2ElfElement = reader.IsDBNull(8) ? null : (ElfElement?)reader.GetInt32(8),
            Player1OrcWeapon = reader.IsDBNull(9) ? null : (OrcWeapon?)reader.GetInt32(9),
            Player2OrcWeapon = reader.IsDBNull(10) ? null : (OrcWeapon?)reader.GetInt32(10),
            Player1BeastAttack = reader.IsDBNull(11) ? null : (BeastAttack?)reader.GetInt32(11),
            Player2BeastAttack = reader.IsDBNull(12) ? null : (BeastAttack?)reader.GetInt32(12),
            Player1CurrentHp = reader.GetDouble(13),
            Player2CurrentHp = reader.GetDouble(14),
            Player1MaxHp = reader.GetDouble(15),
            Player2MaxHp = reader.GetDouble(16),
            Distance = reader.GetInt32(17),
            TurnNumber = reader.GetInt32(18),
            ActiveIndex = reader.GetInt32(19),
            Player1Bleeding = reader.GetInt32(20) == 1,
            Player2Bleeding = reader.GetInt32(21) == 1,
            Player1BleedingTurnsLeft = reader.GetInt32(22),
            Player2BleedingTurnsLeft = reader.GetInt32(23),
            Player1PendingHeal = reader.GetDouble(24),
            Player2PendingHeal = reader.GetDouble(25),
            CurrentTurnPlayerName = reader.GetString(26),
            LastSavedAt = savedAt == default ? DateTime.UtcNow : savedAt
        };

        var logJson = reader.IsDBNull(28) ? "[]" : reader.GetString(28);
        saved.LogJson = logJson;

        return saved;
    }
}
