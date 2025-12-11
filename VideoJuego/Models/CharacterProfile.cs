namespace VideoJuego.Models;

//Clase para representar el perfil de un personaje
public class CharacterProfile
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public Race Race { get; set; }
    public int SubType { get; set; }
}
