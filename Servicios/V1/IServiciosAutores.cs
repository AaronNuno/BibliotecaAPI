using BibliotecaAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Servicios.V1
{
    public interface IServiciosAutores
    {
        Task<IEnumerable<AutorDTO>> Get( PaginacionDTO paginacionDTO);
    }
}