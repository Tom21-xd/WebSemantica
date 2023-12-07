namespace WebSemantica.Models
{
    public class Productora
    {
        public string? Id { get; set; }
        public string? Imagen { get; set; }
        public string? Nombre { get; set; }
        public List<Pelicula> peliculas { get; set; }
        public List<Extra> Extras { get; set; }
    }
}
