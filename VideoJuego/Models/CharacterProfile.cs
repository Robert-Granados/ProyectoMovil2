namespace VideoJuego.Models;

//clase para el perfil del personaje
public class CharacterProfile
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public Race Race { get; set; }
    public int SubType { get; set; }
}
