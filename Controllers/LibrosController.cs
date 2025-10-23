using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaAPI.DTOs;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/libros")]

    public class LibrosController : ControllerBase
    {
        private readonly AplicationDBContext context;
        private readonly IMapper mapper;

        public LibrosController(AplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<IEnumerable<LibroDTO>> Get()
        {
            var libros =  await context.Libros.ToListAsync();
            var libroDTO = mapper.Map<IEnumerable<LibroDTO>>(libros);
            return libroDTO;
        }


        [HttpGet("{id:int}", Name = "ObtenerLibro")] // api/libros/id
        public async Task<ActionResult<LibroConAutorDTO>> Get(int id)
        {
            var libro = await context.Libros
                .Include(x => x.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            var libroDTO = mapper.Map<LibroConAutorDTO>(libro);

            return libroDTO;

        }


        [HttpPost]  // api/libros
        public async Task<ActionResult<LibroDTO>> Post(LibroCreacionDTO libroCreacionDTO)
        {
            var libro = mapper.Map<Libro>(libroCreacionDTO);

            var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);

            if (!existeAutor)
            {
                ModelState.AddModelError(nameof(libro.AutorId), $"El autor de id {libro.AutorId} no existe");
                return ValidationProblem();
            }
            context.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("ObtenerLibro", new  { id = libro.Id }, libroDTO);
        }


        [HttpPut ("{id:int}")] // api/libros/id
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {

            var libro = mapper.Map<Libro>(libroCreacionDTO);
         
            libro.Id = id;

            var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);

            if (!existeAutor)
            {
                return BadRequest($" el autor del id {libro.AutorId} no existe");
            }


            context.Update(libro);
            await context.SaveChangesAsync();   
            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Libro>> Delete (int id)
        {
            var registrosBorrados = await context.Libros.Where(x => x.Id == id).ExecuteDeleteAsync();
            if (registrosBorrados == 0)
            {
                return NotFound();
            }
            return NoContent();

        }




    }
}
