using Microsoft.Data.Sqlite;
using VideoJuego.Models;

namespace VideoJuego.Services;

public class CharacterRepository : ICharacterRepository
{
    private readonly IDatabaseService _database;

    public CharacterRepository(IDatabaseService database)
    {
        _database = database;
    }

    public async Task SaveOrUpdateAsync(int playerId, CharacterConfig config)
    {
        await _database.InitializeAsync();
        await using var connection = _database.GetConnection();
        await connection.OpenAsync();

        var existing = await GetByPlayerIdInternalAsync(connection, playerId);
        var subType = GetSubTypeValue(config);

        if (existing is null)
        {
            await using var insert = connection.CreateCommand();
            insert.CommandText = "INSERT INTO Characters(PlayerId, Race, SubType) VALUES ($player, $race, $sub);";
            insert.Parameters.AddWithValue("$player", playerId);
            insert.Parameters.AddWithValue("$race", (int)config.Race);
            insert.Parameters.AddWithValue("$sub", subType);
            await insert.ExecuteNonQueryAsync();
        }
        else
        {
            await using var update = connection.CreateCommand();
            update.CommandText = "UPDATE Characters SET Race = $race, SubType = $sub WHERE PlayerId = $player;";
            update.Parameters.AddWithValue("$player", playerId);
            update.Parameters.AddWithValue("$race", (int)config.Race);
            update.Parameters.AddWithValue("$sub", subType);
            await update.ExecuteNonQueryAsync();
        }
    }

    public async Task<CharacterProfile?> GetByPlayerIdAsync(int playerId)
    {
        await _database.InitializeAsync();
        await using var connection = _database.GetConnection();
        await connection.OpenAsync();
        return await GetByPlayerIdInternalAsync(connection, playerId);
    }

    private static async Task<CharacterProfile?> GetByPlayerIdInternalAsync(SqliteConnection connection, int playerId)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, PlayerId, Race, SubType FROM Characters WHERE PlayerId = $id;";
        command.Parameters.AddWithValue("$id", playerId);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new CharacterProfile
            {
                Id = reader.GetInt32(0),
                PlayerId = reader.GetInt32(1),
                Race = (Race)reader.GetInt32(2),
                SubType = reader.GetInt32(3)
            };
        }

        return null;
    }

    private static int GetSubTypeValue(CharacterConfig config)
    {
        return config.Race switch
        {
            Race.Human => (int)(config.HumanWeapon ?? Models.HumanWeapon.Shotgun),
            Race.Elf => (int)(config.ElfElement ?? Models.ElfElement.Fire),
            Race.Orc => (int)(config.OrcWeapon ?? Models.OrcWeapon.Axe),
            Race.Beast => (int)(config.BeastAttack ?? Models.BeastAttack.Punches),
            _ => 0
        };
    }
}
