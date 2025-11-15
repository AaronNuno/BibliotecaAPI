using BibliotecaAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    public class RootController :ControllerBase
    {
        [HttpGet(Name="ObtenerRootV1")]
        public IEnumerable <DatosHATEOASDTO> Get()
        {
            var datosHATEOAS = new List<DatosHATEOASDTO>();

            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerRootV1", new { })!,
                              Descripcion: "self", Metodo: "GET"));

            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerAutoresV1", new { })!,
                             Descripcion: "autores-obtener", Metodo: "GET"));

            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerAutorV1", new { })!,
                 Descripcion: "autor-obtener", Metodo: "GET"));

            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("CrearAutorV1", new { })!,
                 Descripcion: "autor-crear", Metodo: "POST"));

            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("CrearAutorConFotoV1", new { })!,
     Descripcion: "autorConFoto-crear", Metodo: "POST"));

            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerLibrosV1", new { })!,
    Descripcion: "lIbros-Obtener", Metodo: "GET"));

            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("RegistroUsuarioV1", new { })!,
               Descripcion: "usuario-registrar", Metodo: "POST"));

            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("LoginUsuarioV1", new { })!,
                Descripcion: "usuario-login", Metodo: "POST"));


            return datosHATEOAS;
        }
    }
}
