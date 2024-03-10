using Application.FichaDomino;
using Domain;
using Domain.FichaDomino;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NewRelic.Api.Agent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPIFichasDomino.APM.NewRelicAgent;

namespace WebAPIFichasDomino.Controllers
{
    [ApiController]
    [Route("api/fichasDomino")]
    public class FichasDominoController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        public FichasDominoController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpPost("ordenarFichas")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<List<FichaDominoEntity>> PostOrdenarFichas([FromBody] RequestData requestData)
        {
            var fichaDominoApplication = new FichaDominoApplication();
            try
            {
                var mensajeValidacion = "Resultado Valido.";
                var listaFichasDesordenadas = fichaDominoApplication.SerializerListaFichasDomino(requestData.fichasDesordenadas);
                
                if (listaFichasDesordenadas == null || listaFichasDesordenadas.Count == 0)
                {
                    mensajeValidacion = "No se ha cargado una lista de fichas.";
                    ReportingNewRelic(mensajeValidacion, requestData.fichasDesordenadas, false);
                    return BadRequest(new ResponseData
                    {
                        fichasDomino = requestData.fichasDesordenadas,
                        validacion = mensajeValidacion
                    });
                }
                if (listaFichasDesordenadas.Count < 2 || listaFichasDesordenadas.Count > 6)
                {
                    mensajeValidacion = "El conjunto de fichas debe tener mínimo dos y máximo seis.";
                    ReportingNewRelic(mensajeValidacion, requestData.fichasDesordenadas, false);
                    return BadRequest(new ResponseData
                    {
                        fichasDomino = requestData.fichasDesordenadas,
                        validacion = mensajeValidacion
                    });
                }

                var listaFichaDominoOrdenado = fichaDominoApplication.Ordenar(listaFichasDesordenadas);

                var fichasResultado = fichaDominoApplication.FormatearFichasDomino(listaFichaDominoOrdenado);

                ReportingNewRelic(mensajeValidacion, requestData.fichasDesordenadas, true, fichasResultado);
                return Ok(new ResponseData
                {
                    fichasDomino = fichasResultado,
                    validacion = mensajeValidacion
                });
            }
            catch(FormatException ex)
            {
                var mensajeValidacion = "La cadena de fichas ingresadas no es correcta.";
                ReportingNewRelic(mensajeValidacion, requestData.fichasDesordenadas, false);
                return BadRequest(new ResponseData
                {
                    fichasDomino = requestData.fichasDesordenadas,
                    validacion = mensajeValidacion
                });
            }
            catch (InvalidOperationException ex)
            {
                var mensajeValidacion = ex.Message;
                ReportingNewRelic(mensajeValidacion, requestData.fichasDesordenadas, false);
                return BadRequest(new ResponseData
                {
                    fichasDomino = requestData.fichasDesordenadas,
                    validacion = mensajeValidacion
                });
            }
            catch (Exception ex)
            {
                var mensajeValidacion = ex.Message;
                ReportingNewRelic(mensajeValidacion, requestData.fichasDesordenadas, false);
                return BadRequest(new ResponseData
                {
                    fichasDomino = requestData.fichasDesordenadas,
                    validacion = mensajeValidacion
                });
            }
        }

        [HttpGet("solicitarToken")]
        public ActionResult<ResponseAuthentication> GetTokenJwt()
        {
            var claimsProject = new List<Claim>()
            {
                new Claim("client", "Inalambria"),
                new Claim("developer", "Juan Polania")
            };

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["LlaveJWT"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expirationJtw = DateTime.UtcNow.AddMinutes(30);

            var securityToken = new JwtSecurityToken(issuer: "inalamabria", audience: null, claims: claimsProject, 
                expires: expirationJtw, signingCredentials: creds);

            return new ResponseAuthentication()
            { Expiration = expirationJtw, Token = new JwtSecurityTokenHandler().WriteToken(securityToken) };
        }

        [Transaction]
        private void ReportingNewRelic(string? message, string? fichasRecibidas, Boolean status, string? fichasOrdenadas = "")
        {
            IApmHandler apm = new NewRelicAPM();
            apm.AddCustomAttribute("client", "Inalambria");
            apm.AddCustomAttribute("message", message);
            apm.AddCustomAttribute("fichasRecibidas", fichasRecibidas);
            apm.AddCustomAttribute("fichasOrdenadas", fichasOrdenadas);
            apm.AddCustomAttribute("status", status.ToString());
        }
    }

}

