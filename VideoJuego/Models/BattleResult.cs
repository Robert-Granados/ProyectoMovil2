namespace VideoJuego.Models;

//clase para almacenar el resultado de la batalla
public class BattleResult
{
    public string WinnerName { get; set; } = string.Empty;
    public double WinnerHealth { get; set; }
    public CharacterConfig WinnerCharacter { get; set; } = new();
    public CharacterConfig Player1Character { get; set; } = new();
    public CharacterConfig Player2Character { get; set; } = new();
    public string Player1Name { get; set; } = string.Empty;
    public string Player2Name { get; set; } = string.Empty;
    public int Turns { get; set; }
    public bool WasDraw { get; set; }
}
