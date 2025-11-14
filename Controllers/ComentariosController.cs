using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    [Authorize]
    public class ComentariosController: ControllerBase
    {
        private readonly AplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IServiciosUsuarios serviciosUsuario;
        private readonly IOutputCacheStore outputCacheStore;
        private const string cache = "comentarios-obtener";

        public ComentariosController(AplicationDBContext context, IMapper mapper ,
                                      IServiciosUsuarios serviciosUsuario,IOutputCacheStore outputCacheStore)
        {
            this.context = context;
            this.mapper = mapper;
            this.serviciosUsuario = serviciosUsuario;
            this.outputCacheStore = outputCacheStore;
        }

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var existeLibro  = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentarios = await context.Comentarios
                .Include(x=> x.Usuario)    
                .Where(x => x.LibroId == libroId)
                .OrderByDescending(x => x.FechaPublicacion)
                .ToListAsync();

            return mapper.Map<List<ComentarioDTO>>(comentarios);
      
        }

        [HttpGet("{id}", Name = "ObetenerComentario")]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
        public async Task<ActionResult<ComentarioDTO>> Get (Guid id)
        {
            var comentario = await context.Comentarios
                .Include (x=> x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (comentario is null)
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTO>(comentario);

        }

        [HttpPost]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }
            var usuario = await serviciosUsuario.ObetenerUsuario();

            if (usuario is null)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;
            comentario.FechaPublicacion = DateTime.UtcNow;
            comentario.UsuarioId = usuario.Id;
            context.Add(comentario);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ObetenerComentario", new {id = comentario.Id, libroId }, comentarioDTO);

        }
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(Guid id, int libroId, JsonPatchDocument<ComentarioPatchDTO> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest();
            }

            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var usuario = await serviciosUsuario.ObetenerUsuario();

            if (usuario is null)
            {
                return NotFound();
            }

            var comentarioDB = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);

            if (comentarioDB.UsuarioId != usuario.Id)
            { 
                return Forbid();
            }



            if (comentarioDB is null)
            {
                return NotFound();
            }

            var comentarioPatchDTO = mapper.Map<ComentarioPatchDTO>(comentarioDB);

            patchDoc.ApplyTo(comentarioPatchDTO, ModelState);

            var esValido = TryValidateModel(comentarioPatchDTO);

            if (!esValido)
            {
                return ValidationProblem();
            }

            mapper.Map(comentarioPatchDTO, comentarioDB);

            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);


            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete (Guid id, int libroId)
        {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var usuario = await serviciosUsuario.ObetenerUsuario();

            if (usuario is null)
            {
                return NotFound();
            }

            var comentarioDB = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);

            if (comentarioDB is null)
            {
                return NotFound();
            }

            if (comentarioDB.UsuarioId != usuario.Id)
            {
                return Forbid();
            }

            comentarioDB.EstaBorrado = true;
            context.Update(comentarioDB);

            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);

            return NoContent();


        }

    }
}
