namespace WebSemantica.Models
{
    public class Pelicula
    {
        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public DateOnly FechaLanzamiento { get; set; }
        public string? Presupuesto { get; set; }
        public string? Recaudacion { get; set;}
        public string? Pais { get; set; }
        public string? Puntuacion { get; set; } 
        public string? UrlImagen { get; set; }
        public List<Actor> actores { get; set; }
        public List<Extra> extra { get; set; }
        public List<Genero> generos { get; set; }
        public Productora productora { get; set; }
        public List<Director> directors { get; set; }
        public string? preview { get; set; }

    }
}
