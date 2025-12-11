using Microsoft.Data.Sqlite;

namespace VideoJuego.Services;

public interface IDatabaseService
{
    string DatabasePath { get; }
    Task InitializeAsync();
    SqliteConnection GetConnection();
}
