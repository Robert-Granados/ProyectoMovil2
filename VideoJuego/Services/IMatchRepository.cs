using VideoJuego.Models;

namespace VideoJuego.Services;

public interface IMatchRepository
{
    Task AddAsync(MatchRecord record);
}
