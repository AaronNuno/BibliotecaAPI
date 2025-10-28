using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/autores-coleccion")]
    public class AutoresColeccionControler: ControllerBase
    {
        private readonly AplicationDBContext context;
        private readonly IMapper mapper;

        public AutoresColeccionControler(AplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> Post(IEnumerable<AutorCreacionDTO> autoresCreacionDTO)
        {
            var autores = mapper.Map<IEnumerable<Autor>>(autoresCreacionDTO);
            context.AddRange(autores);
            await context.SaveChangesAsync();
            return Ok();
            
        }


    }
}
