using System.Collections.Generic;
using VideoJuego.Models;

namespace VideoJuego.Services;

public interface ISaveGameService
{
    Task SaveGameAsync(SavedGame state);
    Task<SavedGame?> LoadGameAsync();
    Task<SavedGame?> LoadGameAsync(int id);
    Task<List<SavedGame>> GetAllAsync();
    Task<bool> HasSavedGameAsync();
    Task DeleteSavedGameAsync();
}
