using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        [HttpGet]

        public IEnumerable<Autor> Get()
        {
            return new List<Autor>()
            {
                new Autor() { Id = 1, Nombre = "Felipe" },
                new Autor() { Id = 2, Nombre = "Claudia" },
                new Autor() { Id = 3, Nombre = "Autor 3" }
            };
        }
    }
}
