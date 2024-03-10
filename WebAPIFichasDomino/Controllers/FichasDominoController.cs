using Application.FichaDomino;
using Domain;
using Domain.FichaDomino;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIFichasDomino.Controllers
{
    [ApiController]
    [Route("api/fichasDomino")]
    public class FichasDominoController : ControllerBase
    {
        [HttpPost("ordenarFichas")]
        public ActionResult<List<FichaDominoEntity>> PostOrdenarFichas([FromBody] RequestData requestData)
        {
            var fichaDominoApplication = new FichaDominoApplication();
            try
            {
                var listaFichasDesordenadas = fichaDominoApplication.SerializerListaFichasDomino(requestData.fichasDesordenadas);
                
                if (listaFichasDesordenadas == null || listaFichasDesordenadas.Count == 0)
                {
                    return NoContent();
                }
                if (listaFichasDesordenadas.Count < 2 || listaFichasDesordenadas.Count > 6)
                {
                    return BadRequest(new ResponseData
                    {
                        fichasDomino = requestData.fichasDesordenadas,
                        validacion = "El conjunto de fichas debe tener mínimo dos y máximo seis."
                    });
                }

                var listaFichaDominoOrdenado = fichaDominoApplication.Ordenar(listaFichasDesordenadas);

                var fichasResultado = fichaDominoApplication.FormatearFichasDomino(listaFichaDominoOrdenado);

                return Ok(new ResponseData
                {
                    fichasDomino = fichasResultado,
                    validacion = "Resultado Valido."
                });
            }
            catch(FormatException ex)
            {
                return BadRequest(new ResponseData
                {
                    fichasDomino = requestData.fichasDesordenadas,
                    validacion = "La cadena de fichas ingresadas no es correcta."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseData
                {
                    fichasDomino = requestData.fichasDesordenadas,
                    validacion = ex.Message
                });
            }
        }

    }
}
