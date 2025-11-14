using Microsoft.AspNetCore.Mvc.Filters;

namespace BibliotecaAPI.Utilidades
{
    public class FiltroAgregarCabeceraAttribute :ActionFilterAttribute
    {
        private readonly string nombre;
        private readonly string valor;

        public FiltroAgregarCabeceraAttribute(string nombre,string valor)
        {
            this.nombre = nombre;
            this.valor = valor;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            //Antes de la ejecución de la acción
            context.HttpContext.Response.Headers.Append(nombre, valor);
            // Después de la ejecución de la acción
            base.OnResultExecuting(context);
        }
    }
}
