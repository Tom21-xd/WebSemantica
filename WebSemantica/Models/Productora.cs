namespace WebSemantica.Models
{
    public class Productora
    {
        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public List<Pelicula> peliculas { get; set; }
    }
}
