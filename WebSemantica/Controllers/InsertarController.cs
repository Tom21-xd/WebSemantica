using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using VDS.RDF.Query;
using VDS.RDF.Query.Paths;
using WebSemantica.Models;

namespace WebSemantica.Controllers
{
    public class InsertarController : Controller
    {
        public SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://localhost:3030/Peliculas/"));

        public IActionResult FormularioPelicula()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Pelicula(Pelicula peli)
        {
            try
            {
                SparqlResultSet resultado = endpoint.QueryWithResultSet(
                    "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
                    "PREFIX dato: <http://www.semanticweb.org/johan/ontologies/2023/10/Cine#> " +
                    "SELECT ?pe " +
                    "WHERE {" +
                        "?pe rdf:type dato:Pelicula. " +
                    "}" +
                    "order by desc(?pe)" +
                    "limit 1"
                );

                var dato = resultado.Results.Single().ToList();
                string id = dato[0].ToString().Split('#')[1].Replace("]", "");

                id = (int.Parse(id.Substring(8, id.Length - 8)) + 1).ToString();

                var fechaCon = (peli.FechaLanzamiento.ToString()).Substring(6, 4) + "-" +
                               (peli.FechaLanzamiento.ToString()).Substring(3, 2) + "-" +
                               (peli.FechaLanzamiento.ToString()).Substring(0, 2) + "T00:00:00";

                    

                resultado = endpoint.QueryWithResultSet(
                    "PREFIX xml: <http://www.w3.org/XML/1998/namespace/#> " +
                    "PREFIX xsd: <http://www.w3.org/2001/XMLSchema#> " +
                    "PREFIX owl: <http://www.w3.org/2002/07/owl#> " +
                    "PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> " +
                    "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
                    "PREFIX dato: <http://www.semanticweb.org/johan/ontologies/2023/10/Cine#> " +
                    "insert data {  " +
                        "dato:Pelicula6 rdf:type dato:Pelicula;" +
                    " }"
                    );
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("FormularioPelicula", "Insertar");
        }
    }
}
