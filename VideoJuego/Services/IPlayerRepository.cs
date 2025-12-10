using VideoJuego.Models;

namespace VideoJuego.Services;

public interface IPlayerRepository
{
    Task<PlayerProfile> GetOrCreateAsync(string name);
    Task<List<PlayerProfile>> GetAllAsync();
    Task UpdateStatsAsync(int playerId, bool won, bool draw);
}
