using VideoJuego.Models;

namespace VideoJuego.Services;

public interface ICharacterRepository
{
    Task SaveOrUpdateAsync(int playerId, CharacterConfig config);
    Task<CharacterProfile?> GetByPlayerIdAsync(int playerId);
}
