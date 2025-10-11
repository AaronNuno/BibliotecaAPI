using BibliotecaAPI.Datos;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/libros")]

    public class LibrosController : ControllerBase
    {
        private readonly AplicationDBContext context;
        public LibrosController(AplicationDBContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<IEnumerable<Libro>> Get()
        {
            return await context.Libros.ToListAsync();
        }
        [HttpGet("{id:int}")] // api/libros/id
        public async Task<ActionResult<Libro>> Get(int id)
        {
            var libro = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);
            if (libro == null) 
            {
                return NotFound();
            }
            return Ok(libro);

        }
        [HttpPost]  // api/libros
        public async Task<ActionResult<Libro>> Post(Libro libro)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            if (!existeAutor)
            {
                return BadRequest($"El autor de id{libro.AutorId} no exixte");
            }
            context.Add(libro);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut ("{id:int}")] // api/libros/id
        public async Task<ActionResult<Libro>> Put(int id, Libro libro)
        {
            if (id != libro.Id)
            {
                return BadRequest("THE ID'S MUST MATCH");
            }
            context.Update(libro);
            await context.SaveChangesAsync();   
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Libro>> Delete (int id)
        {
            var registrosBorrados = await context.Libros.Where(x => x.Id == id).ExecuteDeleteAsync();
            if (registrosBorrados == 0)
            {
                return NotFound();
            }
            return Ok();

        }




    }
}
