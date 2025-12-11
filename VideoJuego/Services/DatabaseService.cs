using Microsoft.Data.Sqlite;
using Microsoft.Maui.Storage;
using System.Collections.Generic;

namespace VideoJuego.Services;

public class DatabaseService : IDatabaseService
{
    private bool _initialized;

    public string DatabasePath { get; } = Path.Combine(FileSystem.AppDataDirectory, "fantasy_duel.db3");

    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath)!);

        await using var connection = GetConnection();
        await connection.OpenAsync();

        var createPlayers = @"CREATE TABLE IF NOT EXISTS Players(
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Name TEXT NOT NULL UNIQUE,
                                Wins INTEGER NOT NULL,
                                Losses INTEGER NOT NULL,
                                Draws INTEGER NOT NULL);";

        var createCharacters = @"CREATE TABLE IF NOT EXISTS Characters(
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                PlayerId INTEGER NOT NULL,
                                Race INTEGER NOT NULL,
                                SubType INTEGER NOT NULL,
                                FOREIGN KEY(PlayerId) REFERENCES Players(Id));";

        var createMatches = @"CREATE TABLE IF NOT EXISTS Matches(
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Date TEXT NOT NULL,
                                Player1Id INTEGER NOT NULL,
                                Player2Id INTEGER NOT NULL,
                                WinnerPlayerId INTEGER NULL,
                                WasDraw INTEGER NOT NULL,
                                Player1Name TEXT NOT NULL,
                                Player2Name TEXT NOT NULL,
                                Player1Race INTEGER NOT NULL,
                                Player2Race INTEGER NOT NULL,
                                Player1Variant TEXT NOT NULL,
                                Player2Variant TEXT NOT NULL,
                                FOREIGN KEY(Player1Id) REFERENCES Players(Id),
                                FOREIGN KEY(Player2Id) REFERENCES Players(Id));";

        var createSavedGames = @"CREATE TABLE IF NOT EXISTS SavedGames(
                                    Id INTEGER PRIMARY KEY CHECK(Id=1),
                                    Player1Name TEXT NOT NULL,
                                    Player2Name TEXT NOT NULL,
                                    Player1Race INTEGER NOT NULL,
                                    Player2Race INTEGER NOT NULL,
                                    Player1HumanWeapon INTEGER NULL,
                                    Player2HumanWeapon INTEGER NULL,
                                    Player1ElfElement INTEGER NULL,
                                    Player2ElfElement INTEGER NULL,
                                    Player1OrcWeapon INTEGER NULL,
                                    Player2OrcWeapon INTEGER NULL,
                                    Player1BeastAttack INTEGER NULL,
                                    Player2BeastAttack INTEGER NULL,
                                    Player1CurrentHp REAL NOT NULL,
                                    Player2CurrentHp REAL NOT NULL,
                                    Player1MaxHp REAL NOT NULL,
                                    Player2MaxHp REAL NOT NULL,
                                    Distance INTEGER NOT NULL,
                                    TurnNumber INTEGER NOT NULL,
                                    ActiveIndex INTEGER NOT NULL,
                                    Player1Bleeding INTEGER NOT NULL DEFAULT 0,
                                    Player2Bleeding INTEGER NOT NULL DEFAULT 0,
                                    Player1BleedingTurnsLeft INTEGER NOT NULL DEFAULT 0,
                                    Player2BleedingTurnsLeft INTEGER NOT NULL DEFAULT 0,
                                    Player1PendingHeal REAL NOT NULL DEFAULT 0,
                                    Player2PendingHeal REAL NOT NULL DEFAULT 0,
                                    CurrentTurnPlayerName TEXT NOT NULL,
                                    LastSavedAt TEXT NOT NULL,
                                    LogJson TEXT NULL);";

        foreach (var commandText in new[] { createPlayers, createCharacters, createMatches, createSavedGames })
        {
            await using var command = connection.CreateCommand();
            command.CommandText = commandText;
            await command.ExecuteNonQueryAsync();
        }

        await EnsureMatchSchemaAsync(connection);
        await EnsureSavedGameSchemaAsync(connection);

        _initialized = true;
    }

    public SqliteConnection GetConnection()
    {
        return new SqliteConnection($"Data Source={DatabasePath}");
    }

    private static async Task EnsureMatchSchemaAsync(SqliteConnection connection)
    {
        var columns = await GetColumnsAsync(connection, "Matches");
        var alterStatements = new List<string>();

        void AddColumnIfMissing(string name, string definition)
        {
            if (!columns.Contains(name))
            {
                alterStatements.Add($"ALTER TABLE Matches ADD COLUMN {name} {definition};");
            }
        }

        AddColumnIfMissing("Player1Name", "TEXT NOT NULL DEFAULT ''");
        AddColumnIfMissing("Player2Name", "TEXT NOT NULL DEFAULT ''");
        AddColumnIfMissing("Player1Race", "INTEGER NOT NULL DEFAULT 0");
        AddColumnIfMissing("Player2Race", "INTEGER NOT NULL DEFAULT 0");
        AddColumnIfMissing("Player1Variant", "TEXT NOT NULL DEFAULT ''");
        AddColumnIfMissing("Player2Variant", "TEXT NOT NULL DEFAULT ''");

        foreach (var statement in alterStatements)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = statement;
            await command.ExecuteNonQueryAsync();
        }
    }

    private static async Task<HashSet<string>> GetColumnsAsync(SqliteConnection connection, string tableName)
    {
        var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        await using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({tableName});";

        await using var reader = await command.ExecuteReaderAsync();
        var nameOrdinal = reader.GetOrdinal("name");

        while (await reader.ReadAsync())
        {
            columns.Add(reader.GetString(nameOrdinal));
        }

        return columns;
    }

    private static async Task EnsureSavedGameSchemaAsync(SqliteConnection connection)
    {
        var columns = await GetColumnsAsync(connection, "SavedGames");
        var alterStatements = new List<string>();

        void AddColumnIfMissing(string name, string definition)
        {
            if (!columns.Contains(name))
            {
                alterStatements.Add($"ALTER TABLE SavedGames ADD COLUMN {name} {definition};");
            }
        }

        AddColumnIfMissing("Player1Name", "TEXT NOT NULL DEFAULT ''");
        AddColumnIfMissing("Player2Name", "TEXT NOT NULL DEFAULT ''");
        AddColumnIfMissing("Player1Race", "INTEGER NOT NULL DEFAULT 0");
        AddColumnIfMissing("Player2Race", "INTEGER NOT NULL DEFAULT 0");
        AddColumnIfMissing("Player1HumanWeapon", "INTEGER NULL");
        AddColumnIfMissing("Player2HumanWeapon", "INTEGER NULL");
        AddColumnIfMissing("Player1ElfElement", "INTEGER NULL");
        AddColumnIfMissing("Player2ElfElement", "INTEGER NULL");
        AddColumnIfMissing("Player1OrcWeapon", "INTEGER NULL");
        AddColumnIfMissing("Player2OrcWeapon", "INTEGER NULL");
        AddColumnIfMissing("Player1BeastAttack", "INTEGER NULL");
        AddColumnIfMissing("Player2BeastAttack", "INTEGER NULL");
        AddColumnIfMissing("Player1CurrentHp", "REAL NOT NULL DEFAULT 0");
        AddColumnIfMissing("Player2CurrentHp", "REAL NOT NULL DEFAULT 0");
        AddColumnIfMissing("Player1MaxHp", "REAL NOT NULL DEFAULT 0");
        AddColumnIfMissing("Player2MaxHp", "REAL NOT NULL DEFAULT 0");
        AddColumnIfMissing("Distance", "INTEGER NOT NULL DEFAULT 0");
        AddColumnIfMissing("TurnNumber", "INTEGER NOT NULL DEFAULT 1");
        AddColumnIfMissing("ActiveIndex", "INTEGER NOT NULL DEFAULT 0");
        AddColumnIfMissing("Player1Bleeding", "INTEGER NOT NULL DEFAULT 0");
        AddColumnIfMissing("Player2Bleeding", "INTEGER NOT NULL DEFAULT 0");
        AddColumnIfMissing("Player1BleedingTurnsLeft", "INTEGER NOT NULL DEFAULT 0");
        AddColumnIfMissing("Player2BleedingTurnsLeft", "INTEGER NOT NULL DEFAULT 0");
        AddColumnIfMissing("Player1PendingHeal", "REAL NOT NULL DEFAULT 0");
        AddColumnIfMissing("Player2PendingHeal", "REAL NOT NULL DEFAULT 0");
        AddColumnIfMissing("CurrentTurnPlayerName", "TEXT NOT NULL DEFAULT ''");
        AddColumnIfMissing("LastSavedAt", "TEXT NOT NULL DEFAULT ''");
        AddColumnIfMissing("LogJson", "TEXT NULL");

        foreach (var statement in alterStatements)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = statement;
            await command.ExecuteNonQueryAsync();
        }
    }
}
