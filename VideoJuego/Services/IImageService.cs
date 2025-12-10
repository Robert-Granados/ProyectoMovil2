using VideoJuego.Models;

namespace VideoJuego.Services;

public interface IImageService
{
    string GetImageForCharacter(CharacterConfig config);
}
