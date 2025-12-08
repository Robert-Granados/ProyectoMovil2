namespace VideoJuego.Models;

public class PlayerProfile
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
}
