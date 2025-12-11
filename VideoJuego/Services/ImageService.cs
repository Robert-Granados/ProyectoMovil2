using VideoJuego.Models;

namespace VideoJuego.Services;

public class ImageService : IImageService
{
    public string GetImageForCharacter(CharacterConfig config)
    {
        return config.Race switch
        {
            Race.Human => config.HumanWeapon == HumanWeapon.SniperRifle
                ? "humano_francotirador.jpg"
                : "humano_escopeta.jpg",
            Race.Elf => config.ElfElement switch
            {
                ElfElement.Fire => "elfo_fuego.jpg",
                ElfElement.Earth => "elfo_tierra.jpg",
                ElfElement.Air => "elfo_aire.jpg",
                ElfElement.Water => "elfo_agua.jpg",
                _ => "elfo_fuego.jpg"
            },
            Race.Orc => config.OrcWeapon == OrcWeapon.Hammer
                ? "orco_martillo.jpg"
                : "orco_hacha.jpg",
            Race.Beast => config.BeastAttack == BeastAttack.Sword
                ? "bestia_espada.jpg"
                : "bestia_punos.jpg",
            _ => "human.png"
        };
    }
}
