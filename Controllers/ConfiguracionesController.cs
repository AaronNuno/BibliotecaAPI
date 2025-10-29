using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/configuracion")]
    public class ConfiguracionesController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ConfiguracionesController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        [HttpGet]
        public ActionResult<string> Get() 
            {
            var opcion1 = configuration["apellido"];

            var opcion2 = configuration.GetValue<string>("apellido")!;

            return opcion2;

            }

        [HttpGet("secciones")]
        public ActionResult<string>GetSeccion()
        {
            var opcion1 = configuration["ConnectionStrings:DefaultConnection"];

            var opcion2 = configuration.GetValue<string>("ConnectionStrings");

            var seccion = configuration.GetSection("ConnectionStrings");

            var opcion3 = seccion["DefaultConnection"];

            return opcion3!;
        }



    }
}
