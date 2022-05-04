using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]

    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService) 
        {
            this.authorizationService = authorizationService;
        }
            
       
        [HttpGet(Name ="obtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatosHATEOAS>>> Get()
        {
            var datosHATEOAS = new List<DatosHATEOAS>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

            //ENLACES GENERALES DE LA API//
            datosHATEOAS.Add(new DatosHATEOAS(enlace: Url.Link("obtenerRoot", new { }), 
                descripcion: "self", metodo: "GET"));

            datosHATEOAS.Add(new DatosHATEOAS(enlace: Url.Link("obtenerAutores", new { }), 
                descripcion: "autores", metodo: "GET"));

            if (esAdmin.Succeeded)
            {
                datosHATEOAS.Add(new DatosHATEOAS(enlace: Url.Link("crearAutor", new { }),
              descripcion: "autor-crear", metodo: "POST"));

                datosHATEOAS.Add(new DatosHATEOAS(enlace: Url.Link("crearLibro", new { }),
                    descripcion: "libro-crear", metodo: "POST"));
            }
            
          
            return datosHATEOAS;

        }
    }
}
