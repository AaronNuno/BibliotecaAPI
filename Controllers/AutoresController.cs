using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using BibliotecaAPI.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize(Policy = "esadmin")]
    public class AutoresController : ControllerBase
    {
        private readonly AplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private const string contenedor = "autores";

        public AutoresController(AplicationDBContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet("/listaAutores")] //lista atuores
        [HttpGet]          //api/autores
        [AllowAnonymous]
        public async Task<IEnumerable<AutorDTO>> Get([FromQuery]  PaginacionDTO paginacionDTO)
        {
            var queryable =  context.Autores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var autores = await queryable.OrderBy(x=> x.Nombres).Paginar(paginacionDTO).ToListAsync();
            var autoresDTO = mapper.Map<IEnumerable<AutorDTO>>(autores);
            
            return autoresDTO;
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



        [HttpGet("{id:int}", Name = "ObtenerAutor")] // api/autores/id
        [AllowAnonymous]
        [EndpointSummary("Obtiene Autor por ID")]
        [EndpointDescription("Obtiene un autor por su ID. Incluye sus libros. Si el autor no existe, se retorna 404")]
        [ProducesResponseType<AutorConLibrosDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AutorConLibrosDTO>> Get([Description("El ID del Autor")]int id)
        {
            var autor = await context.Autores
                .Include(x => x.Libros)
                .ThenInclude(x => x.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (autor is null)
            {
                return NotFound();
            }
            var autorDTO = mapper.Map<AutorConLibrosDTO>(autor);
            return autorDTO;
        }


        [HttpPost]
        public async Task<ActionResult> Post(AutorCreacionDTO autorCreacionDTO)
        {
            var autor = mapper.Map<Autor>(autorCreacionDTO);
            context.Add(autor);
            await context.SaveChangesAsync();
            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("ObtenerAutor", new { id = autor.Id }, autorDTO);
        }


        [HttpPost("con-foto")]
        public async Task<ActionResult> PostConFoto([FromForm]AutorCreacionDTOConFoto autorCreacionDTO)
        {
            var autor = mapper.Map<Autor>(autorCreacionDTO);

            if(autorCreacionDTO.Foto is null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, autorCreacionDTO.Foto);
                autor.Foto =url;
            }

            context.Add(autor);
            await context.SaveChangesAsync();
            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("ObtenerAutor", new { id = autor.Id }, autorDTO);
        }



        [HttpPut("{id:int}")] // api/autores/id
        public async Task<ActionResult> Put(int id,[FromForm] AutorCreacionDTOConFoto autorCreacionDTO)
        {
            var existeAutor = await context.Autores.AnyAsync(x=> x.Id == id);

            if(!existeAutor)
            {
                return NotFound();
            }


            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;

            if(autorCreacionDTO.Foto is not null)
            {
                var fotoActual = await context.Autores.Where(x=> x.Id == id).Select(x => x.Foto).FirstAsync();

                var url = await almacenadorArchivos.Editar(fotoActual, contenedor, autorCreacionDTO.Foto);
                autor.Foto = url;
            }


            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<AutorPatchDTO> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest();
            }
            var autorDB = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autorDB is null)
            {
                return NotFound();
            }
            var autorPatchDTO = mapper.Map<AutorPatchDTO>(autorDB);

            patchDoc.ApplyTo(autorPatchDTO, ModelState);

            var esValido = TryValidateModel(autorPatchDTO);

            if (!esValido)
            {
                return ValidationProblem();
            }

            mapper.Map(autorPatchDTO,autorDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")] // api/autores/id
       public async Task<ActionResult> Delete(int id)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x=> x.Id == id);

            if (autor is null)
            {
                return NotFound();
            }

            context.Remove(autor);
            await context.SaveChangesAsync();
            await almacenadorArchivos.Borrar(autor.Foto, contenedor);

            return NoContent();
        }

    }
}
