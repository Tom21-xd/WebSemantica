using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.X86;
using System.Web;
using VDS.RDF.Query;
using VDS.RDF.Query.Paths;
using WebSemantica.Models;

namespace WebSemantica.Controllers
{
    public class PeliculaController : Controller
    {
        public SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://localhost:3030/Peliculas/"));

        [HttpPost]
        [HttpGet]
        public IActionResult CatalogoPeli(string Nom)
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
                    "SELECT ?np ?desc ?pe ?fecha ?pre ?rec ?pa ?pun ?url " +
                    "WHERE {" +
                        "?pe rdf:type dato:Pelicula." +
                        "?pe dato:Nombre ?np." +
                        "?pe dato:Descripcion ?desc." +
                        "?pe dato:FechaLanzamiento ?fecha." +
                        "?pe dato:Presupuesto ?pre." +
                        "?pe dato:Recaudacion ?rec." +
                        "?pe dato:Pais ?pa." +
                        "?pe dato:Puntuacion ?pun." +
                        "?pe dato:Imagen ?url." +
                        (string.IsNullOrEmpty(Nom) ? "" : $"FILTER(contains(?np, lcase('{Nom}')))") +
                    "}"
                    
                );
                foreach (var pe in resultado.Results)
                {
                    var dato = pe.ToList();
                    pelis.Add(new Pelicula()
                    {
                        Nombre = dato[0].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", ""),
                        Descripcion = dato[1].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", ""),
                        preview = dato[1].Value.ToString().Substring(0, 100) +"...",
                        Id = dato[2].Value.ToString()+"",
                        FechaLanzamiento = DateOnly.Parse(dato[3].Value.ToString().Substring(0,10).Replace("T"," ")),
                        Presupuesto = dato[4].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#float", "") +" Millones USD",
                        Recaudacion = dato[5].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#float", "") +" Millones USD",
                        Pais = dato[6].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", ""),
                        Puntuacion = dato[7].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#float", ""),
                        UrlImagen = dato[8].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#anyURI", "")
                    });
                }
            }catch(Exception ex)
            {

            }
            return View(pelis);
        }

        public IActionResult DetallePeli(string id)
        {
            id = HttpUtility.UrlDecode(id);
            Pelicula aux = new Pelicula();
            try
            {
                SparqlResultSet resultado = endpoint.QueryWithResultSet(
                    "PREFIX xml: <http://www.w3.org/XML/1998/namespace/#> " +
                    "PREFIX xsd: <http://www.w3.org/2001/XMLSchema#> " +
                    "PREFIX owl: <http://www.w3.org/2002/07/owl#> " +
                    "PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> " +
                    "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
                    "PREFIX dato: <http://www.semanticweb.org/johan/ontologies/2023/10/Cine#> " +
                    "SELECT ?np ?desc ?pe ?fecha ?pre ?rec ?pa ?pun ?url " +
                    "WHERE {" +
                        "<"+id+"> rdf:type dato:Pelicula." +
                        "<" + id + "> dato:Nombre ?np." +
                        "<" + id + "> dato:Descripcion ?desc." +
                        "<" + id + "> dato:FechaLanzamiento ?fecha." +
                        "<" + id + "> dato:Presupuesto ?pre." +
                        "<" + id + "> dato:Recaudacion ?rec." +
                        "<" + id + "> dato:Pais ?pa." +
                        "<" + id + "> dato:Puntuacion ?pun . " +
                        "<" + id + "> dato:Imagen ?url . " +
                    "}"
                );
                var dato = resultado.Results.Single().ToList();
                aux.Nombre = dato[0].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                aux.Descripcion = dato[1].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                aux.preview = dato[1].Value.ToString().Substring(0, 100) + "...";
                aux.Id = id;
                aux.FechaLanzamiento = DateOnly.Parse(dato[2].Value.ToString().Substring(0, 10).Replace("T", " "));
                aux.Presupuesto = dato[3].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#float", "") + " Millones USD";
                aux.Recaudacion = dato[4].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#float", "") + " Millones USD";
                aux.Pais = dato[5].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                aux.Puntuacion = dato[6].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#float", "");
                aux.UrlImagen = dato[7].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#anyURI", "");
            }
            catch (Exception ex)
            {

            }
            return View(aux);
        }

        [HttpPost]
        [HttpGet]
        public IActionResult Puntuacion(string anio1, string anio2,string ordenar)
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
                    "SELECT ?imagen ?Pel ?fecha ?punt " +
                    "WHERE {" +
                        "?pe rdf:type dato:Pelicula." +
                        "?pe dato:Imagen ?imagen." +
                        "?pe dato:Nombre ?Pel." +
                        "?pe dato:Puntuacion ?punt ." +
                        "?pe dato:FechaLanzamiento ?fecha." +
                        (string.IsNullOrEmpty(anio1) ? "" : $"FILTER(YEAR(?fecha) >= {anio1})") +
                        (string.IsNullOrEmpty(anio2) ? "" : $"FILTER(YEAR(?fecha) <= {anio2})") +
                    "}" +
                    (string.IsNullOrEmpty(ordenar) ? "" : (ordenar == "Puntuacion") ? "ORDER by desc(?punt)" : "ORDER by desc(?fecha)")
                );
                foreach (var pe in resultado.Results)
                {
                    var dato = pe.ToList();
                    pelis.Add(new Pelicula()
                    {
                        UrlImagen = dato[0].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#anyURI", ""),
                        Nombre = dato[1].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", ""),
                        FechaLanzamiento = DateOnly.Parse(dato[2].Value.ToString().Substring(0, 10).Replace("T", " ")),
                        Puntuacion = dato[3].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#float", ""),
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return View(pelis);
        }

        [HttpPost]
        [HttpGet]
        public IActionResult Genero(string genero,string director)
        {

            List<Pelicula> pelis = new List<Pelicula>();
            try
            {
                SparqlResultSet resultado = endpoint.QueryWithResultSet(
                    
                    "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
                    "PREFIX dato: <http://www.semanticweb.org/johan/ontologies/2023/10/Cine#> " +
                    "SELECT ?imagen ?NombrePelicula (group_concat(?NombreGenero; separator=',') as ?generos) (group_concat(distinct?NombreDirector; separator=',') as ?directores)\r\n" +
                    "WHERE {" +
                        "?pelicula rdf:type dato:Pelicula." +
                        "?pelicula dato:Imagen ?imagen. " +
                        "?pelicula dato:Nombre ?NombrePelicula. " +
                        "?pelicula dato:Pertenece_a  ?a. " +
                        "?a dato:Nombre ?NombreGenero. " +
                        "?pelicula dato:Es_Dirigida_Por ?d. " +
                        "?d dato:Nombre ?NombreDirector. " +
                        (string.IsNullOrEmpty(genero) ? "" : $"filter(contains(lcase(?NombreGenero), lcase('{genero}')))") +
                        (string.IsNullOrEmpty(director) ? "" : $"filter(contains(lcase(?NombreDirector), lcase('{director}')))") +

                    "}" +
                    "group by ?imagen ?NombrePelicula"

                );
                var i = 0;
                foreach (var pe in resultado.Results)
                {
                    var dato = pe.ToList();
                    pelis.Add(new Pelicula()
                    {
                        UrlImagen = dato[0].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#anyURI", ""),
                        Nombre = dato[1].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "")
                    });
                    List<Genero> aux1 = new List<Genero>();
                    var generos = dato[2].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "").Split(",");
                    for(var j=0; j<generos.Length; j++)
                    {
                        aux1.Add(new Genero()
                        {
                            Nombre = generos[j].ToString(),
                        });         
                    }
                    pelis[i].generos = aux1;
                    List<Director> aux2 = new List<Director>();
                    var director1 = dato[3].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "").Split(",");
                    for (var j = 0; j < director1.Length; j++)
                    {
                        aux2.Add(new Director()
                        {
                            Nombre = director1[j].ToString(),
                        });
                    }
                    pelis[i++].directors = aux2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return View(pelis);
        }

        [HttpPost]
        [HttpGet]
        public IActionResult Actores(string actor,string puntaje,string edad1, string edad2)
        {

            List<Actor> act = new List<Actor>();
            try
            {
                SparqlResultSet resultado = endpoint.QueryWithResultSet(

                    "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
                    "PREFIX dato: <http://www.semanticweb.org/johan/ontologies/2023/10/Cine#> " +
                    "SELECT ?Img ?NomActor (group_concat(?nomPelicula; separator=',') as ?peliculas) " +
                    "WHERE {" +
                        "?Actor rdf:type dato:Actores. " +
                        "?Actor dato:Imagen ?Img. " +
                        "?Actor dato:Nombre ?NomActor. " +
                        "?Actor dato:Edad ?Edad. " +
                        "?Actor dato:Actua_En  ?Pelicula. " +
                        "?Pelicula dato:Nombre ?nomPelicula.  " +
                        "?Pelicula dato:Puntuacion ?pun. "+
                        (string.IsNullOrEmpty(actor) ? "" : $"filter(contains(lcase(?NomActor), lcase('{actor}')))") +
                        (string.IsNullOrEmpty(puntaje) ? "" : $"filter(?pun>={puntaje})") +
                        (string.IsNullOrEmpty(edad1) ? "" : $"filter(?Edad>={edad1})") +
                        (string.IsNullOrEmpty(edad2) ? "" : $"filter(?Edad<={edad2})") +
                    "}" +
                    "group by ?Img ?NomActor "

                );
                var i = 0;
                foreach (var pe in resultado.Results)
                {
                    var dato = pe.ToList();
                    act.Add(new Actor()
                    {
                        imagen = dato[0].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#anyURI", ""),
                        Nombre = dato[1].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "")
                    });
                    List<Pelicula> pelis = new List<Pelicula>();
                    var aux1= dato[2].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "").Split(",");
                    for (var j = 0; j < aux1.Length; j++)
                    {
                        pelis.Add(new Pelicula()
                        {
                            Nombre = aux1[j].ToString(),
                        });
                    }
                    act[i++].peliculas = pelis;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return View(act);
        }


        [HttpPost]
        [HttpGet]
        public IActionResult Productora(string genero, string director)
        {

            /*List<Pelicula> pelis = new List<Pelicula>();
            try
            {
                SparqlResultSet resultado = endpoint.QueryWithResultSet(

                    "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
                    "PREFIX dato: <http://www.semanticweb.org/johan/ontologies/2023/10/Cine#> " +
                    "SELECT ?imagen ?NombrePelicula (group_concat(?NombreGenero; separator=',') as ?generos) (group_concat(distinct?NombreDirector; separator=',') as ?directores)\r\n" +
                    "WHERE {" +
                        "?pelicula rdf:type dato:Pelicula." +
                        "?pelicula dato:Imagen ?imagen. " +
                        "?pelicula dato:Nombre ?NombrePelicula. " +
                        "?pelicula dato:Pertenece_a  ?a. " +
                        "?a dato:Nombre ?NombreGenero. " +
                        "?pelicula dato:Es_Dirigida_Por ?d. " +
                        "?d dato:Nombre ?NombreDirector. " +
                        (string.IsNullOrEmpty(genero) ? "" : $"filter(contains(lcase(?NombreGenero), lcase('{genero}')))") +
                        (string.IsNullOrEmpty(director) ? "" : $"filter(contains(lcase(?NombreDirector), lcase('{director}')))") +

                    "}" +
                    "group by ?imagen ?NombrePelicula"

                );
                var i = 0;
                foreach (var pe in resultado.Results)
                {
                    var dato = pe.ToList();
                    pelis.Add(new Pelicula()
                    {
                        UrlImagen = dato[0].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#anyURI", ""),
                        Nombre = dato[1].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "")
                    });
                    List<Genero> aux1 = new List<Genero>();
                    var generos = dato[2].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "").Split(",");
                    for (var j = 0; j < generos.Length; j++)
                    {
                        aux1.Add(new Genero()
                        {
                            Nombre = generos[j].ToString(),
                        });
                    }
                    pelis[i].generos = aux1;
                    List<Director> aux2 = new List<Director>();
                    var director1 = dato[3].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "").Split(",");
                    for (var j = 0; j < director1.Length; j++)
                    {
                        aux2.Add(new Director()
                        {
                            Nombre = director1[j].ToString(),
                        });
                    }
                    pelis[i++].directors = aux2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }*/
            return View();
        }



    }
}
