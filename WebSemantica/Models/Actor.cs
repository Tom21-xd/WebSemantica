﻿namespace WebSemantica.Models
{
    public class Actor
    {
        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public int Edad { get; set; }
        public List<Pelicula> peliculas { get; set; }
    }
}
