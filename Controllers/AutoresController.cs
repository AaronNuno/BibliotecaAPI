using BibliotecaAPI.Datos;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly AplicationDBContext context;
        public AutoresController(AplicationDBContext context)
        {
            this.context = context;
        }

        [HttpGet("/listaAutores")] //lista atuores
        [HttpGet]          //api/autores
        public async Task<IEnumerable<Autor>> Get()
        {
            return await context.Autores.ToListAsync();
        } 

        [HttpGet("{nombre:alpha}")] 
        public async Task<IEnumerable<Autor>> Get(string nombre)
        {
            return await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();
        }
        

        /*[HttpGet("{parametro}/{parametro2?}")] //api/autores/aaron/nuno
        public ActionResult Get(string parametro, string? parametro2 )
        {
            return Ok(new {parametro,parametro2});
        }*/

        [HttpGet("primero")] // api/autores/primero
        public async   Task<Autor> GetPrimerAutor()

        {
            return await context.Autores.FirstAsync();
        }

        [HttpGet("{id:int}")] // api/autores/id
        public async Task<ActionResult<Autor>>  Get(int id)
        {
            var autor = await context.Autores
                .Include(x=>x.Libros)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (autor is null)
            {
                return NotFound();
            }
            return autor;
        }
        

        [HttpPost]
        public async Task<ActionResult> Post(Autor autor)
        {
            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }


        [HttpPut ("{id:int}")] // api/autores/id
        public async Task<ActionResult> Put(int id, Autor autor)
        {
            if (id != autor.Id)
            {
                return BadRequest("THE ID'S MUST MATCH");
            }
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
