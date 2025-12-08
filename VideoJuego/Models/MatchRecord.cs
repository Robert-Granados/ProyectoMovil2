namespace VideoJuego.Models;

public class MatchRecord
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int Player1Id { get; set; }
    public int Player2Id { get; set; }
    public int? WinnerPlayerId { get; set; }
    public bool WasDraw { get; set; }
    public string Player1Name { get; set; } = string.Empty;
    public string Player2Name { get; set; } = string.Empty;
    public Race Player1Race { get; set; }
    public Race Player2Race { get; set; }
    public string Player1Variant { get; set; } = string.Empty;
    public string Player2Variant { get; set; } = string.Empty;
}
