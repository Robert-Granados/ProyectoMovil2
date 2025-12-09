using System.Globalization;
using Microsoft.Maui.Controls;
using VideoJuego.Models;

namespace VideoJuego.Converters;

public class EnumToSpanishConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Race race)
        {
            return race switch
            {
                Race.Human => "Humano",
                Race.Elf => "Elfo",
                Race.Orc => "Orco",
                Race.Beast => "Bestia",
                _ => race.ToString()
            };
        }

        if (value is HumanWeapon human)
        {
            return human switch
            {
                HumanWeapon.Shotgun => "Escopeta",
                HumanWeapon.SniperRifle => "Rifle francotirador",
                _ => human.ToString()
            };
        }

        if (value is ElfElement element)
        {
            return element switch
            {
                ElfElement.Fire => "Fuego",
                ElfElement.Earth => "Tierra",
                ElfElement.Air => "Aire",
                ElfElement.Water => "Agua",
                _ => element.ToString()
            };
        }

        if (value is OrcWeapon orcWeapon)
        {
            return orcWeapon switch
            {
                OrcWeapon.Axe => "Hacha",
                OrcWeapon.Hammer => "Martillo",
                _ => orcWeapon.ToString()
            };
        }

        if (value is BeastAttack beastAttack)
        {
            return beastAttack switch
            {
                BeastAttack.Punches => "Puños",
                BeastAttack.Sword => "Espada",
                _ => beastAttack.ToString()
            };
        }

        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value ?? string.Empty;
    }
}
