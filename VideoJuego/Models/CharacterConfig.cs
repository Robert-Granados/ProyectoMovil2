namespace VideoJuego.Models;

//clase para configurar un personaje
public class CharacterConfig
{
    public Race Race { get; set; }
    public HumanWeapon? HumanWeapon { get; set; }
    public ElfElement? ElfElement { get; set; }
    public OrcWeapon? OrcWeapon { get; set; }
    public BeastAttack? BeastAttack { get; set; }

    public string Describe()
    {
        return Race switch
        {
            Race.Human => $"Humano - {(HumanWeapon == Models.HumanWeapon.Shotgun ? "Escopeta" : "Rifle francotirador")}",
            Race.Elf => $"Elfo - {ElfElement}",
            Race.Orc => $"Orco - {(OrcWeapon == Models.OrcWeapon.Axe ? "Hacha" : "Martillo")}",
            Race.Beast => $"Bestia - {(BeastAttack == Models.BeastAttack.Punches ? "Puños" : "Espada")}",
            _ => Race.ToString()
        };
    }
}
