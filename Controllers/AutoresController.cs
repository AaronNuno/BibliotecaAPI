using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly AplicationDBContext context;
        private readonly IMapper mapper;

        public AutoresController(AplicationDBContext context, IMapper mapper )
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("/listaAutores")] //lista atuores
        [HttpGet]          //api/autores
        public async Task<IEnumerable<AutorDTO>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            var autoresDTO = mapper.Map<IEnumerable<AutorDTO>>(autores);
            //var autoresDTO = autores.Select(autor => new AutorDTO { Id = autor.Id, NombreCompleto = $"{autor.Nombres} {autor.Apellidos}" });
            return   autoresDTO;
        } 

        [HttpGet("{nombre:alpha}")] 
        public async Task<IEnumerable<Autor>> Get(string nombre)
        {
            return await context.Autores.Where(x => x.Nombres.Contains(nombre)).ToListAsync();
        }
        

        /*[HttpGet("{parametro}/{parametro2?}")] //api/autores/aaron/nuno
        public ActionResult Get(string parametro, string? parametro2 )
        {
            return Ok(new {parametro,parametro2});
        }*/



        [HttpGet("{id:int}", Name = "ObtenerAutor") ] // api/autores/id
        public async Task<ActionResult<AutorDTO>>  Get(int id)
        {
            var autor = await context.Autores
                .Include(x=>x.Libros)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (autor is null)
            {
                return NotFound();
            }
            var autorDTO = mapper.Map<AutorDTO>(autor);
            return autorDTO;
        }
        

        [HttpPost]
        public async Task<ActionResult> Post(AutorCreacionDTO autorCreacionDTO)
        {
            var autor = mapper.Map<Autor>(autorCreacionDTO);
            context.Add(autor);
            await context.SaveChangesAsync();
            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("ObtenerAutor", new {id = autor.Id},autorDTO);
        }


        [HttpPut ("{id:int}")] // api/autores/id
        public async Task<ActionResult> Put(int id, AutorCreacionDTO autorCreacionDTO)
        {
            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;
            context.Update(autor);  
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")] // api/autores/id
       public async Task<ActionResult> Delete(int id)
        {
            var registrosBorrados = await context.Autores.Where(x => x.Id == id).ExecuteDeleteAsync();
            if (registrosBorrados == 0)
            {
                return NotFound();
            }
            return Ok();
        }

    }
}
