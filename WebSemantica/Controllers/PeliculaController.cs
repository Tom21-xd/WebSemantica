using Microsoft.AspNetCore.Mvc;
using VDS.RDF.Query;
using WebSemantica.Models;

namespace WebSemantica.Controllers
{
    public class PeliculaController : Controller
    {
        public SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://localhost:3030/Peliculas/"));
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CatalogoPeli()
        {
            List<Pelicula> pelis = new List<Pelicula>();
            try
            {
                SparqlResultSet resultado = endpoint.QueryWithResultSet(
                    "PREFIX xml: <http://www.w3.org/XML/1998/namespace/#> " +
                    "PREFIX xsd: <http://www.w3.org/2001/XMLSchema#> " +
                    "PREFIX owl: <http://www.w3.org/2002/07/owl#> " +
                    "PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> " +
                    "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
                    "PREFIX dato: <http://www.semanticweb.org/johan/ontologies/2023/10/Cine#> " +
                    "SELECT  ?nombre ?desc ?pelicula ?fecha " +
                    "WHERE { " +
                        "?pelicula rdf:type dato:Pelicula. " +
                        "?pelicula dato:Nombre ?nombre. " +
                        "?pelicula dato:Descripcion ?desc." +
                        "?pelicula dato:FechaLanzamiento ?fecha "+
                    "} "
                );
                foreach (var pe in resultado.Results)
                {
                    var dato = pe.ToList();
                    pelis.Add(new Pelicula()
                    {
                        Nombre = dato[0].Value.ToString(),
                        Descripcion = dato[1].Value.ToString(),
                        preview = dato[1].Value.ToString().Substring(0, 100),
                        Id = dato[2].Value.ToString()
                    });
                }
            }catch(Exception ex)
            {

            }
           

            return View(pelis);
        }
    }
}
