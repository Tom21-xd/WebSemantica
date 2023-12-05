namespace WebSemantica.Models
{
    public class Genero
    {
        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public List<Pelicula> peliculas { get; set; }
    }
}
