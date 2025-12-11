namespace VideoJuego.Models;

public class Combatant
{
    public string PlayerName { get; set; } = string.Empty;
    public CharacterConfig Config { get; set; } = new();
    public double MaxHealth { get; set; }
    public double CurrentHealth { get; set; }
    public int BleedTurnsLeft { get; set; }
    public double PendingHealNextTurn { get; set; }
    public string ImageResource { get; set; } = string.Empty;

    public bool IsAlive => CurrentHealth > 0;

    public void ApplyDamage(double damage)
    {
        CurrentHealth -= damage;
    }

    public void Heal(double amount)
    {
        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);
    }
}
