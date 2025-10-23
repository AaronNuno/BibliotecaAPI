using AutoMapper;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;

namespace BibliotecaAPI.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Autor, AutorDTO>()
                .ForMember(
                    dto => dto.NombreCompleto,
                    opt => opt.MapFrom(autor => MapearNombreYApellidoAutor(autor))
                );

            CreateMap<Autor, AutorConLibrosDTO>()
             .ForMember(
                dto => dto.NombreCompleto,
                opt => opt.MapFrom(autor => MapearNombreYApellidoAutor(autor))
    );

            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<Libro, LibroDTO>();
            CreateMap<LibroCreacionDTO, Libro>();

            CreateMap<Libro, LibroConAutorDTO>()
                .ForMember(dto => dto.AutorNombre, config => 
                config.MapFrom(ent => MapearNombreYApellidoAutor(ent.Autor!)));
        }

        private string MapearNombreYApellidoAutor(Autor autor) => $"{autor.Nombres} {autor.Apellidos}";
    }
}
