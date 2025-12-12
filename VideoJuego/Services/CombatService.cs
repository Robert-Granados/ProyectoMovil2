using VideoJuego.Models;

namespace VideoJuego.Services;

public class CombatService : ICombatService
{
    private readonly Random _random = new();
    private readonly IImageService _imageService;

    public CombatService(IImageService imageService)
    {
        _imageService = imageService;
    }

    public CombatState StartBattle(string player1, CharacterConfig config1, string player2, CharacterConfig config2)
    {
        var combatant1 = CreateCombatant(player1, config1);
        var combatant2 = CreateCombatant(player2, config2);

        return new CombatState
        {
            Combatants = new List<Combatant> { combatant1, combatant2 },
            ActiveIndex = 0,
            Distance = 2,
            TurnNumber = 1
        };
    }

    public CombatActionResult PerformAction(CombatState state, CombatActionType action)
    {
        if (state.IsFinished)
        {
            return new CombatActionResult { Message = "La batalla ya terminó.", TurnConsumed = false, BattleEnded = true };
        }

        CombatActionResult result = action switch
        {
            CombatActionType.Advance => HandleAdvance(state),
            CombatActionType.Retreat => HandleRetreat(state),
            CombatActionType.Heal => HandleHeal(state),
            CombatActionType.HumanShotgun => HandleHumanShotgun(state),
            CombatActionType.HumanSniper => HandleHumanSniper(state),
            CombatActionType.ElfSpell => HandleElfSpell(state),
            CombatActionType.OrcAxe => HandleOrcAxe(state),
            CombatActionType.OrcHammer => HandleOrcHammer(state),
            CombatActionType.BeastPunches => HandleBeastPunches(state),
            CombatActionType.BeastSword => HandleBeastSword(state),
            _ => new CombatActionResult { Message = "Acción desconocida.", TurnConsumed = false }
        };

        if (!string.IsNullOrWhiteSpace(result.Message))
        {
            state.Log.Add($"T{state.TurnNumber} - {result.Message}");
        }

        if (result.BattleEnded)
        {
            state.IsFinished = true;
            state.WinnerName = result.Winner?.PlayerName;
            return result;
        }

        if (result.TurnConsumed)
        {
            var winner = CheckForWinner(state);
            if (winner is not null)
            {
                state.IsFinished = true;
                state.WinnerName = winner.PlayerName;
                return new CombatActionResult
                {
                    Message = $"{winner.PlayerName} ha ganado la batalla.",
                    BattleEnded = true,
                    Winner = winner
                };
            }

            AdvanceTurn(state);
        }

        return result;
    }

    private Combatant CreateCombatant(string player, CharacterConfig config)
    {
        var maxHealth = GetBaseHealth(config);
        return new Combatant
        {
            PlayerName = player,
            Config = config,
            MaxHealth = maxHealth,
            CurrentHealth = maxHealth,
            ImageResource = _imageService.GetImageForCharacter(config)
        };
    }

    private double GetBaseHealth(CharacterConfig config)
    {
        if (config.Race == Race.Elf && config.ElfElement == ElfElement.Water)
        {
            return 115;
        }

        return 100;
    }

    private CombatActionResult HandleAdvance(CombatState state)
    {
        if (state.Distance == 0)
        {
            return new CombatActionResult { Message = $"{state.Active.PlayerName} ya está en distancia cuerpo a cuerpo.", TurnConsumed = false };
        }

        state.Distance = Math.Max(0, state.Distance - 1);
        return new CombatActionResult { Message = $"{state.Active.PlayerName} avanza. Distancia ahora: {state.Distance}.", TurnConsumed = true };
    }

    private CombatActionResult HandleRetreat(CombatState state)
    {
        if (state.Distance == 2)
        {
            return new CombatActionResult { Message = $"{state.Active.PlayerName} ya está a máxima distancia.", TurnConsumed = false };
        }

        state.Distance = Math.Min(2, state.Distance + 1);
        return new CombatActionResult { Message = $"{state.Active.PlayerName} retrocede. Distancia ahora: {state.Distance}.", TurnConsumed = true };
    }

    private CombatActionResult HandleHeal(CombatState state)
    {
        var actor = state.Active;
        var missing = actor.MaxHealth - actor.CurrentHealth;

        if (missing <= 0)
        {
            return new CombatActionResult { Message = $"{actor.PlayerName} ya está a vida completa, la curación se desperdicia.", TurnConsumed = true };
        }

        double healAmount;
        switch (actor.Config.Race)
        {
            case Race.Human:
                healAmount = missing * RandomRange(0.41, 0.49);
                actor.Heal(healAmount);
                return new CombatActionResult
                {
                    Message = $"{actor.PlayerName} come y recupera {healAmount:0} de vida.",
                    TurnConsumed = true
                };
            case Race.Elf:
                if (actor.Config.ElfElement == ElfElement.Water)
                {
                    healAmount = missing * RandomRange(0.75, 0.90);
                    actor.Heal(healAmount);
                    return new CombatActionResult { Message = $"{actor.PlayerName} (Agua) se cura {healAmount:0} de vida.", TurnConsumed = true };
                }

                healAmount = missing * RandomRange(0.65, 0.70);
                actor.Heal(healAmount);
                return new CombatActionResult { Message = $"{actor.PlayerName} usa magia de curación y recupera {healAmount:0} de vida.", TurnConsumed = true };
            case Race.Orc:
                var immediate = missing * RandomRange(0.25, 0.45);
                var extraNext = missing * RandomRange(0.05, 0.25);
                actor.Heal(immediate);
                actor.PendingHealNextTurn = extraNext;
                return new CombatActionResult
                {
                    Message = $"{actor.PlayerName} bebe poción y cura {immediate:0} ahora. Curará {extraNext:0} al inicio de su próximo turno.",
                    TurnConsumed = true
                };
            case Race.Beast:
                healAmount = missing * 0.5;
                actor.Heal(healAmount);
                return new CombatActionResult { Message = $"{actor.PlayerName} duerme y recupera {healAmount:0} de vida.", TurnConsumed = true };
            default:
                return new CombatActionResult { Message = "Raza no soportada para curar.", TurnConsumed = false };
        }
    }

    private CombatActionResult HandleHumanShotgun(CombatState state)
    {
        if (state.Distance > 0)
        {
            return new CombatActionResult { Message = "La escopeta solo funciona a distancia cercana.", TurnConsumed = false };
        }

        var attacker = state.Active;
        var defender = state.Opponent;
        if (TryDodge(defender, state.Distance))
        {
            return new CombatActionResult { Message = $"{defender.PlayerName} esquiva gracias a la magia de Aire.", TurnConsumed = true };
        }
        var damage = _random.Next(1, 6) + _random.Next(0, 3);
        defender.ApplyDamage(damage);

        return new CombatActionResult
        {
            Message = $"{attacker.PlayerName} dispara la escopeta e inflige {damage} de daño.",
            TurnConsumed = true,
            BattleEnded = defender.CurrentHealth <= 0,
            Winner = defender.CurrentHealth <= 0 ? attacker : null
        };
    }

    private CombatActionResult HandleHumanSniper(CombatState state)
    {
        var attacker = state.Active;
        var defender = state.Opponent;

        if (TryDodge(defender, state.Distance))
        {
            return new CombatActionResult { Message = $"{defender.PlayerName} esquiva gracias a la magia de Aire.", TurnConsumed = true };
        }

        if (state.Distance == 0 && attacker.Config.HumanWeapon != HumanWeapon.SniperRifle)
        {
            // rifle  funcionando de cerca pero con daño bajo.
        }

        int damage = state.Distance == 2 ? _random.Next(10, 21) : _random.Next(1, 6);
        defender.ApplyDamage(damage);

        return new CombatActionResult
        {
            Message = $"{attacker.PlayerName} usa el rifle y hace {damage} de daño.",
            TurnConsumed = true,
            BattleEnded = defender.CurrentHealth <= 0,
            Winner = defender.CurrentHealth <= 0 ? attacker : null
        };
    }

    private CombatActionResult HandleElfSpell(CombatState state)
    {
        var attacker = state.Active;
        var defender = state.Opponent;
        var element = attacker.Config.ElfElement ?? ElfElement.Fire;

        if (state.Distance > 0 && element != ElfElement.Air)
        {
            return new CombatActionResult { Message = "Solo la magia de Aire alcanza a distancia. Acércate o usa Aire.", TurnConsumed = false };
        }

        double damage;
        double hitChance;

        switch (element)
        {
            case ElfElement.Fire:
                damage = RandomRangeInt(10, 16);
                hitChance = 0.80;
                break;
            case ElfElement.Earth:
                damage = RandomRangeInt(8, 13);
                hitChance = 0.92;
                break;
            case ElfElement.Air:
                damage = RandomRangeInt(7, 11);
                hitChance = 0.82;
                if (state.Distance > 0)
                {
                    damage *= RandomRange(1.05, 1.15);
                }

                break;
            case ElfElement.Water:
                damage = RandomRangeInt(7, 11);
                hitChance = 0.85;
                break;
            default:
                damage = RandomRangeInt(7, 11);
                hitChance = 0.80;
                break;
        }

        if (_random.NextDouble() > hitChance)
        {
            return new CombatActionResult { Message = $"{attacker.PlayerName} falla el hechizo.", TurnConsumed = true };
        }

        // Esquiva por aire del defensor
        if (TryDodge(defender, state.Distance))
        {
            return new CombatActionResult { Message = $"{defender.PlayerName} esquiva gracias a la magia de Aire.", TurnConsumed = true };
        }

        defender.ApplyDamage(damage);

        return new CombatActionResult
        {
            Message = $"{attacker.PlayerName} lanza magia de {element} e inflige {damage:0} de daño.",
            TurnConsumed = true,
            BattleEnded = defender.CurrentHealth <= 0,
            Winner = defender.CurrentHealth <= 0 ? attacker : null
        };
    }

    private CombatActionResult HandleOrcAxe(CombatState state)
    {
        if (state.Distance != 0)
        {
            return new CombatActionResult { Message = "El orco necesita estar en cuerpo a cuerpo para usar el hacha.", TurnConsumed = false };
        }

        var attacker = state.Active;
        var defender = state.Opponent;
        var damage = _random.Next(1, 6);

        if (TryDodge(defender, state.Distance))
        {
            return new CombatActionResult { Message = $"{defender.PlayerName} esquiva gracias a la magia de Aire.", TurnConsumed = true };
        }

        defender.ApplyDamage(damage);
        defender.BleedTurnsLeft = 2;

        return new CombatActionResult
        {
            Message = $"{attacker.PlayerName} corta con el hacha, hace {damage} de daño y aplica sangrado.",
            TurnConsumed = true,
            BattleEnded = defender.CurrentHealth <= 0,
            Winner = defender.CurrentHealth <= 0 ? attacker : null
        };
    }

    private CombatActionResult HandleOrcHammer(CombatState state)
    {
        if (state.Distance != 0)
        {
            return new CombatActionResult { Message = "El orco necesita estar en cuerpo a cuerpo para usar el martillo.", TurnConsumed = false };
        }

        var attacker = state.Active;
        var defender = state.Opponent;
        var damage = _random.Next(2, 8);

        if (TryDodge(defender, state.Distance))
        {
            return new CombatActionResult { Message = $"{defender.PlayerName} esquiva gracias a la magia de Aire.", TurnConsumed = true };
        }

        defender.ApplyDamage(damage);

        return new CombatActionResult
        {
            Message = $"{attacker.PlayerName} golpea con martillo por {damage} de daño.",
            TurnConsumed = true,
            BattleEnded = defender.CurrentHealth <= 0,
            Winner = defender.CurrentHealth <= 0 ? attacker : null
        };
    }

    private CombatActionResult HandleBeastPunches(CombatState state)
    {
        if (state.Distance != 0)
        {
            return new CombatActionResult { Message = "La bestia debe estar cerca para golpear.", TurnConsumed = false };
        }

        var attacker = state.Active;
        var defender = state.Opponent;
        if (TryDodge(defender, state.Distance))
        {
            return new CombatActionResult { Message = $"{defender.PlayerName} esquiva gracias a la magia de Aire.", TurnConsumed = true };
        }
        var damage = _random.Next(20, 31);
        defender.ApplyDamage(damage);
        attacker.ApplyDamage(10);

        return new CombatActionResult
        {
            Message = $"{attacker.PlayerName} golpea con puños ({damage} de daño) y se autolesiona 10.",
            TurnConsumed = true,
            BattleEnded = defender.CurrentHealth <= 0 || attacker.CurrentHealth <= 0,
            Winner = defender.CurrentHealth <= 0 ? attacker : attacker.CurrentHealth <= 0 ? defender : null
        };
    }

    private CombatActionResult HandleBeastSword(CombatState state)
    {
        if (state.Distance != 0)
        {
            return new CombatActionResult { Message = "La bestia debe estar cerca para usar la espada.", TurnConsumed = false };
        }

        var attacker = state.Active;
        var defender = state.Opponent;
        if (TryDodge(defender, state.Distance))
        {
            return new CombatActionResult { Message = $"{defender.PlayerName} esquiva gracias a la magia de Aire.", TurnConsumed = true };
        }
        var damage = _random.Next(1, 11);
        defender.ApplyDamage(damage);

        return new CombatActionResult
        {
            Message = $"{attacker.PlayerName} ataca con espada por {damage} de daño.",
            TurnConsumed = true,
            BattleEnded = defender.CurrentHealth <= 0,
            Winner = defender.CurrentHealth <= 0 ? attacker : null
        };
    }

    private bool TryDodge(Combatant defender, int distance)
    {
        if (defender.Config.Race != Race.Elf || defender.Config.ElfElement != ElfElement.Air)
        {
            return false;
        }

        var dodgeChance = 0.25 + (distance > 0 ? 0.05 : 0);
        return _random.NextDouble() < dodgeChance;
    }

    private Combatant? CheckForWinner(CombatState state)
    {
        var activeAlive = state.Active.IsAlive;
        var opponentAlive = state.Opponent.IsAlive;

        if (activeAlive && !opponentAlive)
        {
            return state.Active;
        }

        if (!activeAlive && opponentAlive)
        {
            return state.Opponent;
        }

        return null;
    }

    private void AdvanceTurn(CombatState state)
    {
        state.ActiveIndex = (state.ActiveIndex + 1) % 2;
        state.TurnNumber++;
        var effects = ApplyStartTurnEffects(state.Active);
        if (!string.IsNullOrWhiteSpace(effects))
        {
            state.Log.Add($"T{state.TurnNumber}-inicio: {effects}");
        }

        if (!state.Active.IsAlive)
        {
            state.IsFinished = true;
            state.WinnerName = state.Opponent.PlayerName;
        }
    }

    private string ApplyStartTurnEffects(Combatant combatant)
    {
        var messages = new List<string>();

        if (combatant.BleedTurnsLeft > 0)
        {
            combatant.ApplyDamage(3);
            combatant.BleedTurnsLeft--;
            messages.Add($"{combatant.PlayerName} sufre 3 de sangrado.");
        }

        if (combatant.PendingHealNextTurn > 0)
        {
            var healed = combatant.PendingHealNextTurn;
            combatant.Heal(healed);
            combatant.PendingHealNextTurn = 0;
            messages.Add($"{combatant.PlayerName} recibe curación tardía de {healed:0}.");
        }

        return string.Join(" ", messages);
    }

    private double RandomRange(double min, double max) => min + _random.NextDouble() * (max - min);

    private int RandomRangeInt(int minInclusive, int maxInclusive) => _random.Next(minInclusive, maxInclusive + 1);
}
